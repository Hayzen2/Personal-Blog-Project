namespace DemoNETWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileUrlAndType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentSections", "FileUrl", c => c.String(unicode: false));
            AddColumn("dbo.ContentSections", "FileType", c => c.String(unicode: false));
            DropColumn("dbo.ContentSections", "FilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContentSections", "FilePath", c => c.String(unicode: false));
            DropColumn("dbo.ContentSections", "FileType");
            DropColumn("dbo.ContentSections", "FileUrl");
        }
    }
}
