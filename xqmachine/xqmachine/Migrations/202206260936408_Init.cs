namespace xqmachine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountAddress",
                c => new
                    {
                        Account_address_id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Province_id = c.Int(nullable: false),
                        District_id = c.Int(nullable: false),
                        Ward_id = c.Int(nullable: false),
                        AccountPhoneNumber = c.String(maxLength: 10),
                        AccountUsername = c.String(maxLength: 20),
                        Content = c.String(maxLength: 50),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Account_address_id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Districts", t => t.District_id)
                .ForeignKey("dbo.Provinces", t => t.Province_id)
                .ForeignKey("dbo.Wards", t => t.Ward_id)
                .Index(t => t.UserId)
                .Index(t => t.Province_id)
                .Index(t => t.District_id)
                .Index(t => t.Ward_id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 256),
                        Create_at = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 1),
                        Create_by = c.String(maxLength: 100),
                        Update_by = c.String(maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                        Requestcode = c.String(maxLength: 100),
                        Avatar = c.String(unicode: false, storeType: "text"),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Feedback",
                c => new
                    {
                        Feedback_id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Product_id = c.Int(nullable: false),
                        Genre_id = c.Int(nullable: false),
                        Disscount_id = c.Int(nullable: false),
                        Rate_star = c.Int(nullable: false),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 100, unicode: false),
                        Stastus = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Update_by = c.String(nullable: false, maxLength: 100),
                        Update_at = c.DateTime(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Feedback_id)
                .ForeignKey("dbo.Product", t => new { t.Product_id, t.Genre_id, t.Disscount_id })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => new { t.Product_id, t.Genre_id, t.Disscount_id });
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Product_id = c.Int(nullable: false, identity: true),
                        Genre_id = c.Int(nullable: false),
                        Disscount_id = c.Int(nullable: false),
                        Brand_id = c.Int(nullable: false),
                        Product_name = c.String(nullable: false, maxLength: 200),
                        Price = c.Double(nullable: false),
                        View = c.Long(nullable: false),
                        Buyturn = c.Long(nullable: false),
                        Quantity = c.String(maxLength: 10, unicode: false),
                        Status = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Create_by = c.String(nullable: false, maxLength: 100, unicode: false),
                        Create_at = c.DateTime(nullable: false),
                        Type = c.Int(),
                        Update_by = c.String(maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                        Specifications = c.String(),
                        Image = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => new { t.Product_id, t.Genre_id, t.Disscount_id })
                .ForeignKey("dbo.Brand", t => t.Brand_id)
                .ForeignKey("dbo.Discount", t => t.Disscount_id, cascadeDelete: true)
                .ForeignKey("dbo.Genre", t => t.Genre_id)
                .Index(t => t.Genre_id)
                .Index(t => t.Disscount_id)
                .Index(t => t.Brand_id);
            
            CreateTable(
                "dbo.Brand",
                c => new
                    {
                        Brand_id = c.Int(nullable: false, identity: true),
                        Brand_name = c.String(nullable: false, maxLength: 50),
                        Create_by = c.String(nullable: false, maxLength: 100, unicode: false),
                        Create_at = c.DateTime(nullable: false),
                        Update_by = c.String(nullable: false, maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Brand_id);
            
            CreateTable(
                "dbo.Discount",
                c => new
                    {
                        Disscount_id = c.Int(nullable: false, identity: true),
                        Discount_name = c.String(nullable: false, maxLength: 100),
                        Discount_star = c.DateTime(nullable: false),
                        Discount_end = c.DateTime(nullable: false),
                        Discount_price = c.Double(nullable: false),
                        Discount_code = c.String(maxLength: 10),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 100),
                        Update_by = c.String(nullable: false, maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Disscount_id);
            
            CreateTable(
                "dbo.Genre",
                c => new
                    {
                        Genre_id = c.Int(nullable: false, identity: true),
                        Genre_name = c.String(nullable: false, maxLength: 50),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 100, unicode: false),
                        Update_by = c.String(nullable: false, maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Genre_id);
            
            CreateTable(
                "dbo.Oder_Detail",
                c => new
                    {
                        Product_id = c.Int(nullable: false),
                        Genre_id = c.Int(nullable: false),
                        Disscount_id = c.Int(nullable: false),
                        Order_id = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Status = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Transection = c.String(nullable: false, maxLength: 50),
                        Create_by = c.String(nullable: false, maxLength: 20, unicode: false),
                        Create_at = c.DateTime(nullable: false),
                        Update_by = c.String(nullable: false, maxLength: 20),
                        Update_at = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_id, t.Genre_id, t.Disscount_id, t.Order_id })
                .ForeignKey("dbo.Order", t => t.Order_id)
                .ForeignKey("dbo.Product", t => new { t.Product_id, t.Genre_id, t.Disscount_id })
                .Index(t => new { t.Product_id, t.Genre_id, t.Disscount_id })
                .Index(t => t.Order_id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Order_id = c.Int(nullable: false, identity: true),
                        Payment_id = c.Int(nullable: false),
                        Delivery_id = c.Int(nullable: false),
                        Oder_date = c.DateTime(nullable: false),
                        Total = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                        Status = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 100, unicode: false),
                        Update_by = c.String(nullable: false, maxLength: 100),
                        Update_at = c.DateTime(nullable: false),
                        Order_note = c.String(maxLength: 200),
                        OrderAddressId = c.Int(),
                    })
                .PrimaryKey(t => t.Order_id)
                .ForeignKey("dbo.Delivery", t => t.Delivery_id)
                .ForeignKey("dbo.OrderAddress", t => t.OrderAddressId)
                .ForeignKey("dbo.Payment", t => t.Payment_id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.Payment_id)
                .Index(t => t.Delivery_id)
                .Index(t => t.UserId)
                .Index(t => t.OrderAddressId);
            
            CreateTable(
                "dbo.Delivery",
                c => new
                    {
                        Delivery_id = c.Int(nullable: false, identity: true),
                        Delivery_name = c.String(nullable: false, maxLength: 100),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 20, unicode: false),
                        Status = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Update_by = c.String(nullable: false, maxLength: 20),
                        Update_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.Delivery_id);
            
            CreateTable(
                "dbo.OrderAddress",
                c => new
                    {
                        OrderAddressId = c.Int(nullable: false, identity: true),
                        OrderPhonenumber = c.String(maxLength: 10),
                        OrderUsername = c.String(maxLength: 20),
                        Content = c.String(maxLength: 150),
                        TimesEdit = c.Int(nullable: false),
                        Province_id = c.Int(),
                        District_id = c.Int(),
                        Ward_id = c.Int(),
                    })
                .PrimaryKey(t => t.OrderAddressId)
                .ForeignKey("dbo.Districts", t => t.District_id)
                .ForeignKey("dbo.Provinces", t => t.Province_id)
                .ForeignKey("dbo.Wards", t => t.Ward_id)
                .Index(t => t.Province_id)
                .Index(t => t.District_id)
                .Index(t => t.Ward_id);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        District_id = c.Int(nullable: false, identity: true),
                        Province_id = c.Int(nullable: false),
                        District_name = c.String(nullable: false, maxLength: 50),
                        Type = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.District_id)
                .ForeignKey("dbo.Provinces", t => t.Province_id, cascadeDelete: true)
                .Index(t => t.Province_id);
            
            CreateTable(
                "dbo.Provinces",
                c => new
                    {
                        Province_id = c.Int(nullable: false, identity: true),
                        Province_name = c.String(nullable: false, maxLength: 50),
                        Type = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Province_id);
            
            CreateTable(
                "dbo.Wards",
                c => new
                    {
                        Ward_id = c.Int(nullable: false, identity: true),
                        District_id = c.Int(nullable: false),
                        Ward_name = c.String(nullable: false, maxLength: 50),
                        Type = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Ward_id)
                .ForeignKey("dbo.Districts", t => t.District_id, cascadeDelete: true)
                .Index(t => t.District_id);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        Payment_id = c.Int(nullable: false, identity: true),
                        Payment_name = c.String(nullable: false, maxLength: 50),
                        Create_at = c.DateTime(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 20, unicode: false),
                        Status = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Update_by = c.String(nullable: false, maxLength: 20),
                        Update_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.Payment_id);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Product_img_id = c.Int(nullable: false, identity: true),
                        Product_id = c.Int(nullable: false),
                        Genre_id = c.Int(nullable: false),
                        Disscount_id = c.Int(nullable: false),
                        Image = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Product_img_id)
                .ForeignKey("dbo.Product", t => new { t.Product_id, t.Genre_id, t.Disscount_id }, cascadeDelete: true)
                .Index(t => new { t.Product_id, t.Genre_id, t.Disscount_id });
            
            CreateTable(
                "dbo.ReplyFeedback",
                c => new
                    {
                        Rep_feedback_id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Stastus = c.String(maxLength: 1),
                        Create_at = c.DateTime(nullable: false),
                        Feedback_id = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Rep_feedback_id)
                .ForeignKey("dbo.Feedback", t => t.Feedback_id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.Feedback_id)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        Contact_id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Create_by = c.String(nullable: false, maxLength: 20),
                        Create_at = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 1),
                        Update_by = c.String(nullable: false, maxLength: 20),
                        Update_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.Contact_id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReplyFeedback", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Feedback", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReplyFeedback", "Feedback_id", "dbo.Feedback");
            DropForeignKey("dbo.ProductImages", new[] { "Product_id", "Genre_id", "Disscount_id" }, "dbo.Product");
            DropForeignKey("dbo.Oder_Detail", new[] { "Product_id", "Genre_id", "Disscount_id" }, "dbo.Product");
            DropForeignKey("dbo.Order", "Payment_id", "dbo.Payment");
            DropForeignKey("dbo.Order", "OrderAddressId", "dbo.OrderAddress");
            DropForeignKey("dbo.OrderAddress", "Ward_id", "dbo.Wards");
            DropForeignKey("dbo.Wards", "District_id", "dbo.Districts");
            DropForeignKey("dbo.AccountAddress", "Ward_id", "dbo.Wards");
            DropForeignKey("dbo.OrderAddress", "Province_id", "dbo.Provinces");
            DropForeignKey("dbo.Districts", "Province_id", "dbo.Provinces");
            DropForeignKey("dbo.AccountAddress", "Province_id", "dbo.Provinces");
            DropForeignKey("dbo.OrderAddress", "District_id", "dbo.Districts");
            DropForeignKey("dbo.AccountAddress", "District_id", "dbo.Districts");
            DropForeignKey("dbo.Oder_Detail", "Order_id", "dbo.Order");
            DropForeignKey("dbo.Order", "Delivery_id", "dbo.Delivery");
            DropForeignKey("dbo.Product", "Genre_id", "dbo.Genre");
            DropForeignKey("dbo.Feedback", new[] { "Product_id", "Genre_id", "Disscount_id" }, "dbo.Product");
            DropForeignKey("dbo.Product", "Disscount_id", "dbo.Discount");
            DropForeignKey("dbo.Product", "Brand_id", "dbo.Brand");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AccountAddress", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.ReplyFeedback", new[] { "UserId" });
            DropIndex("dbo.ReplyFeedback", new[] { "Feedback_id" });
            DropIndex("dbo.ProductImages", new[] { "Product_id", "Genre_id", "Disscount_id" });
            DropIndex("dbo.Wards", new[] { "District_id" });
            DropIndex("dbo.Districts", new[] { "Province_id" });
            DropIndex("dbo.OrderAddress", new[] { "Ward_id" });
            DropIndex("dbo.OrderAddress", new[] { "District_id" });
            DropIndex("dbo.OrderAddress", new[] { "Province_id" });
            DropIndex("dbo.Order", new[] { "OrderAddressId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.Order", new[] { "Delivery_id" });
            DropIndex("dbo.Order", new[] { "Payment_id" });
            DropIndex("dbo.Oder_Detail", new[] { "Order_id" });
            DropIndex("dbo.Oder_Detail", new[] { "Product_id", "Genre_id", "Disscount_id" });
            DropIndex("dbo.Product", new[] { "Brand_id" });
            DropIndex("dbo.Product", new[] { "Disscount_id" });
            DropIndex("dbo.Product", new[] { "Genre_id" });
            DropIndex("dbo.Feedback", new[] { "Product_id", "Genre_id", "Disscount_id" });
            DropIndex("dbo.Feedback", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AccountAddress", new[] { "Ward_id" });
            DropIndex("dbo.AccountAddress", new[] { "District_id" });
            DropIndex("dbo.AccountAddress", new[] { "Province_id" });
            DropIndex("dbo.AccountAddress", new[] { "UserId" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Contact");
            DropTable("dbo.ReplyFeedback");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Payment");
            DropTable("dbo.Wards");
            DropTable("dbo.Provinces");
            DropTable("dbo.Districts");
            DropTable("dbo.OrderAddress");
            DropTable("dbo.Delivery");
            DropTable("dbo.Order");
            DropTable("dbo.Oder_Detail");
            DropTable("dbo.Genre");
            DropTable("dbo.Discount");
            DropTable("dbo.Brand");
            DropTable("dbo.Product");
            DropTable("dbo.Feedback");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AccountAddress");
        }
    }
}
