using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DemoNETWeb.Models;


namespace DemoNETWeb.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public List<ContentSection> ContentSections { get; set; } = new List<ContentSection>();
        public DateTime CreatedDate { get; set; }
        public List<PostUpdateDate> UpdateDates { get; set; } = new List<PostUpdateDate>();
    }

    public class ContentSection
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; } 
        public string FileUrl { get; set; }
        public string FileType { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        public virtual Post Post { get; set; }
    }
    public class PostUpdateDate
    {
        [Key]
        public int Id { get; set; }
        public DateTime UpdateDate { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }

}