namespace Lidia.Scheduler.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBInitialization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobCollections",
                c => new
                    {
                        CollectionId = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(),
                        ApplicationId = c.Int(nullable: false),
                        Name = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        Code = c.String(),
                        Domain = c.String(),
                        Name = c.String(),
                        Culture = c.String(),
                        TimeZone = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applications", t => t.ParentId)
                .ForeignKey("dbo.Tenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.AccessKeys",
                c => new
                    {
                        AccessKeyId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        Key = c.String(),
                        Secret = c.String(),
                        Token = c.String(),
                        Permission = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.AccessKeyId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.ApplicationProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Culture = c.String(),
                        TimeZone = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantBillingInformation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantContactInformation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantPaymentInformation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Tenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        TenantId = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        Gender = c.String(),
                        Birthdate = c.DateTime(),
                        MobileNumber = c.String(),
                        MobileNumberConfirmed = c.Boolean(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        UserClaimId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.UserClaimId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        JobId = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Name = c.String(),
                        AppKey = c.String(),
                        Process = c.String(),
                        Description = c.String(),
                        CronExpression = c.String(),
                        JobUrl = c.String(),
                        ProcessResult = c.Boolean(nullable: false),
                        SendSummary = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.JobId)
                .ForeignKey("dbo.JobCollections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId);
            
            CreateTable(
                "dbo.JobLog",
                c => new
                    {
                        EntryId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Summary = c.String(),
                        Details = c.String(),
                        Change = c.String(),
                        ActivityType = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.EntryId)
                .ForeignKey("dbo.Jobs", t => t.JobId, cascadeDelete: true)
                .Index(t => t.JobId);
            
            CreateTable(
                "dbo.JobProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Jobs", t => t.JobId, cascadeDelete: true)
                .Index(t => t.JobId);
            
            CreateTable(
                "dbo.JobTags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.Jobs", t => t.JobId, cascadeDelete: true)
                .Index(t => t.JobId);
            
            CreateTable(
                "dbo.JobCollectionProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.JobCollections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId);
            
            CreateTable(
                "dbo.JobCollectionTags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.JobCollections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.JobCollections", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.JobCollectionTags", "CollectionId", "dbo.JobCollections");
            DropForeignKey("dbo.JobCollectionProperties", "CollectionId", "dbo.JobCollections");
            DropForeignKey("dbo.JobTags", "JobId", "dbo.Jobs");
            DropForeignKey("dbo.JobProperties", "JobId", "dbo.Jobs");
            DropForeignKey("dbo.JobLog", "JobId", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "CollectionId", "dbo.JobCollections");
            DropForeignKey("dbo.JobCollections", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantProperties", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantPaymentInformation", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantContactInformation", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantBillingInformation", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Applications", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ApplicationProperties", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.Applications", "ParentId", "dbo.Applications");
            DropForeignKey("dbo.AccessKeys", "ApplicationId", "dbo.Applications");
            DropIndex("dbo.JobCollectionTags", new[] { "CollectionId" });
            DropIndex("dbo.JobCollectionProperties", new[] { "CollectionId" });
            DropIndex("dbo.JobTags", new[] { "JobId" });
            DropIndex("dbo.JobProperties", new[] { "JobId" });
            DropIndex("dbo.JobLog", new[] { "JobId" });
            DropIndex("dbo.Jobs", new[] { "CollectionId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.UserClaims", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "TenantId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.TenantProperties", new[] { "TenantId" });
            DropIndex("dbo.TenantPaymentInformation", new[] { "TenantId" });
            DropIndex("dbo.TenantContactInformation", new[] { "TenantId" });
            DropIndex("dbo.TenantBillingInformation", new[] { "TenantId" });
            DropIndex("dbo.ApplicationProperties", new[] { "ApplicationId" });
            DropIndex("dbo.AccessKeys", new[] { "ApplicationId" });
            DropIndex("dbo.Applications", new[] { "TenantId" });
            DropIndex("dbo.Applications", new[] { "ParentId" });
            DropIndex("dbo.JobCollections", new[] { "ApplicationId" });
            DropIndex("dbo.JobCollections", new[] { "TenantId" });
            DropTable("dbo.Roles");
            DropTable("dbo.JobCollectionTags");
            DropTable("dbo.JobCollectionProperties");
            DropTable("dbo.JobTags");
            DropTable("dbo.JobProperties");
            DropTable("dbo.JobLog");
            DropTable("dbo.Jobs");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
            DropTable("dbo.TenantProperties");
            DropTable("dbo.TenantPaymentInformation");
            DropTable("dbo.TenantContactInformation");
            DropTable("dbo.TenantBillingInformation");
            DropTable("dbo.Tenants");
            DropTable("dbo.ApplicationProperties");
            DropTable("dbo.AccessKeys");
            DropTable("dbo.Applications");
            DropTable("dbo.JobCollections");
        }
    }
}
