# Blog Platform with ASP.NET MVC

A full-featured blog platform built with ASP.NET MVC, Entity Framework, and MySQL, featuring rich text/image posts and version history tracking.

## Key Features ✨

- **Multi-Section Posts**  
  Create posts with multiple content sections, combining text and images/files.
- **Cloud Image Storage**  
  Integrated with MinIO for secure, S3-compatible file storage.
- **Post Versioning**  
  Track edit history with timestamps, viewable in a modal.
- **Rich Media Support**  
  Upload and display images, documents, and other file types with previews.
- **CRUD Operations**  
  Easily create, read, update, and delete posts.
- **Responsive Design**  
  Mobile-friendly interface powered by Bootstrap 5.

## Tech Stack 🛠️

- **Backend**: ASP.NET MVC 5, Entity Framework 6
- **Database**: MySQL
- **Storage**: MinIO (S3-compatible object storage)
- **Frontend**: Bootstrap 5, jQuery
- **Icons**: Bootstrap Icons
- **Other**: .NET Framework 4.8, AWSSDK.S3

## Installation & Setup ⚙️

### 1. Prerequisites
- **.NET Framework 4.8**: Install via [Microsoft .NET Framework](https://dotnet.microsoft.com/download/dotnet-framework).
- **Visual Studio 2019 or later**: Community, Professional, or Enterprise edition.
- **MySQL Server**: Install MySQL Community Server ([download](https://dev.mysql.com/downloads/mysql/)).
- **MinIO Server**: Set up locally or use a remote instance ([MinIO setup guide](https://min.io/docs/minio/linux/index.html)).
- **Git**: For cloning the repository ([git-scm.com](https://git-scm.com)).

### 2. Clone Repository
```bash
git clone https://github.com/your-username/DemoNETWeb.git
cd DemoNETWeb
```
Replace `your-username` with your GitHub username.

### 3. Restore NuGet Packages
- Open `DemoNETWeb.sln` in Visual Studio.
- Right-click the solution in Solution Explorer and select **Restore NuGet Packages**.
- Key dependencies include:
  - `MySql.Data.EntityFramework`
  - `AWSSDK.S3`
  - `Microsoft.AspNet.Mvc`

### 4. Database Setup
- Configure a MySQL database named `BlogDB`.
- Update the connection string in `Web.config`:
  ```xml
  <connectionStrings>
    <add name="BlogContext"
         connectionString="Server=localhost;Database=BlogDB;Uid=your-username;Pwd=your-password;"
         providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
  ```
  Replace `your-username` and `your-password` with your MySQL credentials.
- Run Entity Framework migrations:
  ```powershell
  Enable-Migrations
  Add-Migration InitialCreate
  Update-Database
  ```
  Use the Package Manager Console in Visual Studio.

### 5. Configure MinIO
- Start a MinIO server (e.g., locally at `http://localhost:9000`).
- Create a bucket (e.g., `posts`) in MinIO.
- Add MinIO settings to `Web.config`:
  ```xml
  <appSettings>
    <add key="MinioEndpoint" value="http://localhost:9000"/>
    <add key="MinioAccessKey" value="your-access-key"/>
    <add key="MinioSecretKey" value="your-secret-key"/>
    <add key="MinioBucket" value="posts"/>
  </appSettings>
  ```
  Replace `your-access-key` and `your-secret-key` with your MinIO credentials.

### 6. Run the Application
- Press **Ctrl+F5** in Visual Studio to build and run.
- The application will launch at `http://localhost:port/` (port varies based on your setup).
- Access the homepage to start creating and managing posts.

## Usage Guide 📖

### Create a Post
- Navigate to `/Home/AddForm`.
- Enter a title and add multiple text sections and file uploads (e.g., images, PDFs).
- Submit to create the post and store files in MinIO.

### Edit a Post
- Go to `/Home/EditPost/{id}`.
- Update the title, add/remove content sections, or replace/delete files.
- Changes are tracked in the post’s version history.

### View a Post
- Access `/Home/ViewPost/{id}` to see the post’s content and files.
- Click **Show Update History** to view edit timestamps in a modal.
- Images are displayed with zoom functionality; other files are downloadable.

### Manage Posts
- **Delete Posts**: Remove posts and their associated MinIO files from the homepage or view page.
- **Responsive Interface**: Use on desktop or mobile devices with a consistent experience.

## Development 🔧

### Key Components
- **Models/Post.cs**: Defines the `Post` entity with title, creation date, update dates, and content sections.
- **Models/ContentSection.cs**: Represents a post section with text and file data.
- **Controllers/HomeController.cs**: Handles CRUD operations, MinIO integration, and version history.
- **Views/Home/**:
  - `EditPost.cshtml`: Dynamic post editor with section management.
  - `ViewPost.cshtml`: Post viewer with update history modal.
  - `Homepage.cshtml`: Lists all posts.
- **Data_Context/BlogContext.cs**: Entity Framework context for MySQL.
- **Migrations/**: Database schema configurations.

### Important Dependencies
- `MySql.Data.EntityFramework`: MySQL database provider for Entity Framework.
- `AWSSDK.S3`: MinIO integration for file storage.
- `Bootstrap 5`: Responsive frontend framework.
- `Bootstrap Icons`: Icon library for UI elements.

### Extending the Project
- Add user authentication with ASP.NET Identity.
- Implement post categories or tags.
- Add search functionality for posts.
