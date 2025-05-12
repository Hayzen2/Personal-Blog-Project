namespace DemoNETWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentSections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(unicode: false),
                        FilePath = c.String(unicode: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContentSections", "PostId", "dbo.Posts");
            DropIndex("dbo.ContentSections", new[] { "PostId" });
            DropTable("dbo.Posts");
            DropTable("dbo.ContentSections");
        }
    }
}
