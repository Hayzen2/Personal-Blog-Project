using DemoNETWeb.Models;
using DemoNETWeb.Data_Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; 
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using System.Configuration;
namespace DemoNETWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogContext _context = new BlogContext();

        public ActionResult Homepage()
        {
            var posts = _context.Posts
                .Include(p => p.ContentSections)
                .ToList();
            return View(posts);
        }
        public ActionResult ViewPost(int id)
        {
            var post = _context.Posts
            .Include(p => p.ContentSections)
            .Include(p => p.UpdateDates) 
            .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        public ActionResult AddForm()
        {
            return View();
        }

        public async Task<ActionResult> Delete(int id)
        {
            var post = _context.Posts
                .Include(p => p.ContentSections)
                .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound("Post not available");
            }
            foreach (var section in post.ContentSections)
            {
                if(section.FileUrl != null)
                {
                    await DeleteFileFromMinIO(section.FileUrl);
                    
                }
            }
            _context.Posts.Remove(post);
            _context.SaveChanges();
            return RedirectToAction("Homepage");
        }

       

        [HttpPost]
        public async Task<ActionResult> Add(string title, IEnumerable<string> content, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            var contentSections = new List<ContentSection>();
            var uploadedFilesList = uploadedFiles?.ToList() ?? new List<HttpPostedFileBase>();

            for (int i = 0; i < content.Count(); i++)
            {
                var section = new ContentSection
                {
                    Text = content.ElementAt(i)
                };

                if (uploadedFilesList.Count > i && uploadedFilesList[i] != null)
                {
                    var file = uploadedFilesList[i];
                    var (url, FileType) = await SaveFileToMinIO(file); 
                    section.FileUrl = url;
                    section.FileType = FileType;

                }
                contentSections.Add(section);
            }

            var post = new Post
            {
                Title = title,
                CreatedDate = DateTime.Now,
                ContentSections = contentSections
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewPost", new { id = post.Id });
        }

        public ActionResult EditPost(int id)
        {
            var post = _context.Posts.Include("ContentSections").FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task <ActionResult> Edit(int id, string title, List<string> newContent, List<HttpPostedFileBase> uploadedFiles, List<int> sectionIds, List<bool> deleteFiles)
        {
            var post = _context.Posts.Include("ContentSections").FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            post.Title = title;
            post.UpdateDates.Add(new PostUpdateDate { UpdateDate = DateTime.Now });
            deleteFiles = deleteFiles ?? new List<bool>();
            for (int i = 0; i < deleteFiles.Count; i++)
            {
                if (i < post.ContentSections.Count && deleteFiles[i])
                {
                    var section = post.ContentSections[i];
                    if (!string.IsNullOrEmpty(section.FileUrl))
                    {
                        await DeleteFileFromMinIO(section.FileUrl);
                        section.FileUrl = null;
                        section.FileType = null;
                    }
                }
            }

            // Delete sections that were removed in the edit
            var currentSectionIds = post.ContentSections.Select(s => s.Id).ToList();
            var removedIds = currentSectionIds.Except(sectionIds).ToList();

            foreach (var removedId in removedIds)
            {
                var section = post.ContentSections.First(s => s.Id == removedId);
                if (section.FileUrl != null)
                {
                    await DeleteFileFromMinIO(section.FileUrl);
                }
                _context.ContentSections.Remove(section);
            }

            // Update existing sections and add new ones
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var sectionId = sectionIds[i];
                ContentSection section;

                // New section (ID = -1 indicates new section)
                if (sectionId == -1)
                {
                    section = new ContentSection { PostId = post.Id };
                    post.ContentSections.Add(section);
                }
                else
                {
                    section = post.ContentSections.First(s => s.Id == sectionId);
                    if (section == null)
                    {
                        continue; // Skip invalid section IDs
                    }
                }

                // Update text content
                section.Text = newContent[i];

                // Handle file upload/replacement
                if (uploadedFiles != null && uploadedFiles.Count > i && uploadedFiles[i] != null)
                {
                    // Delete old file if exists
                    if (section.FileUrl != null)
                    {
                        await DeleteFileFromMinIO(section.FileUrl);
                    }

                    // Upload new file
                    var (url, type) = await SaveFileToMinIO(uploadedFiles[i]);
                    section.FileUrl = url;
                    section.FileType = type;
                }
            }


            await _context.SaveChangesAsync();
            return RedirectToAction("ViewPost", new { id = post.Id });
        }


        [HttpPost]
        private async Task<(string Url, string FileType)> SaveFileToMinIO(HttpPostedFileBase file)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = ConfigurationManager.AppSettings["MinioEndpoint"], // MinIO server address
                ForcePathStyle = true  // Required for MinIO to use path-style URLs
            };
            // Create an S3 client with MinIO credentials
            using (var client = new AmazonS3Client(
                ConfigurationManager.AppSettings["MinioAccessKey"],
                ConfigurationManager.AppSettings["MinioSecretKey"],
                config))  // Pass the MinIO-specific configuration
            {
                // Generate unique filename using GUID + original extension
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Create the file upload request
                var request = new PutObjectRequest
                {
                    BucketName = ConfigurationManager.AppSettings["MinioBucket"],  // Target bucket name
                    Key = fileName,  // Object key (filename in bucket)
                    InputStream = file.InputStream,  // File content stream
                    ContentType = file.ContentType,  // MIME type (e.g., "image/png")
                    CannedACL = S3CannedACL.PublicRead  // Set public read permissions
                };

                // Execute the upload asynchronously
                var response = await client.PutObjectAsync(request)
                    .ConfigureAwait(false);  // Optimize for non-UI context

                return (
                    // Construct URL format: http://minio-endpoint/bucket-name/filename
                    $"{ConfigurationManager.AppSettings["MinioEndpoint"]}/{ConfigurationManager.AppSettings["MinioBucket"]}/{fileName}",
                    file.ContentType
                );
            }
        }

        private async Task DeleteFileFromMinIO(string fileUrl)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = ConfigurationManager.AppSettings["MinioEndpoint"],
                ForcePathStyle = true
            };
            using (var client = new AmazonS3Client(
                ConfigurationManager.AppSettings["MinioAccessKey"],
                ConfigurationManager.AppSettings["MinioSecretKey"],
                config))
            {
                var bucket = ConfigurationManager.AppSettings["MinioBucket"];

                var uri = new Uri(fileUrl);
                var key = uri.AbsolutePath // "/posts/abc123.jpg"
                             .Substring(1) // "posts/abc123.jpg"
                             .Replace($"{bucket}/", ""); // "abc123.jpg"

                await client.DeleteObjectAsync(
                   ConfigurationManager.AppSettings["MinioBucket"],
                   key
                );
            }
        }

    }
}