namespace BatteryCommander.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Qualifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        ParentTaskId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Qualifications", t => t.ParentTaskId)
                .Index(t => t.ParentTaskId);
            
            CreateTable(
                "dbo.SoldierQualifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SoldierId = c.Int(nullable: false),
                        QualificationId = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        QualificationDate = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(),
                        Comments = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Qualifications", t => t.QualificationId, cascadeDelete: true)
                .ForeignKey("dbo.Soldiers", t => t.SoldierId, cascadeDelete: true)
                .Index(t => t.SoldierId)
                .Index(t => t.QualificationId);
            
            CreateTable(
                "dbo.Soldiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        Rank = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        Position = c.Int(nullable: false),
                        SecurityClearance = c.Int(nullable: false),
                        MOS = c.Int(nullable: false),
                        IsDutyMOSQualified = c.Boolean(nullable: false),
                        EducationLevelCompleted = c.Int(nullable: false),
                        Group = c.Int(nullable: false),
                        ETSDate = c.DateTime(),
                        Notes = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        EmailAddressConfirmed = c.Boolean(nullable: false),
                        Password = c.String(nullable: false),
                        SecurityStamp = c.String(),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        LockoutEndDate = c.DateTime(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SoldierQualifications", "SoldierId", "dbo.Soldiers");
            DropForeignKey("dbo.SoldierQualifications", "QualificationId", "dbo.Qualifications");
            DropForeignKey("dbo.Qualifications", "ParentTaskId", "dbo.Qualifications");
            DropIndex("dbo.AppUsers", new[] { "UserName" });
            DropIndex("dbo.SoldierQualifications", new[] { "QualificationId" });
            DropIndex("dbo.SoldierQualifications", new[] { "SoldierId" });
            DropIndex("dbo.Qualifications", new[] { "ParentTaskId" });
            DropTable("dbo.AppUsers");
            DropTable("dbo.Soldiers");
            DropTable("dbo.SoldierQualifications");
            DropTable("dbo.Qualifications");
        }
    }
}
