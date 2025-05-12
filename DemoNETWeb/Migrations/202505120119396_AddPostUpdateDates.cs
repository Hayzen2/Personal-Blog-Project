namespace DemoNETWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPostUpdateDates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostUpdateDates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UpdateDate = c.DateTime(nullable: false, precision: 0),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostUpdateDates", "PostId", "dbo.Posts");
            DropIndex("dbo.PostUpdateDates", new[] { "PostId" });
            DropTable("dbo.PostUpdateDates");
        }
    }
}
