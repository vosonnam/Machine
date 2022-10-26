using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace xqmachine.Models.db
{
    public partial class DataBaseContext : DbContext
    {
        public DataBaseContext()
            : base("name=DbXQMachine")
        {
        }

        public virtual DbSet<AccountAddress> AccountAddresses { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Oder_Detail> Oder_Detail { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderAddress> OrderAddresses { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<ReplyFeedback> ReplyFeedbacks { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .Property(e => e.Avatar)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AccountAddresses)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Feedbacks)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.ReplyFeedbacks)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Brand>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Brand>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Brand)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Delivery>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Delivery>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Delivery>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Delivery>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Delivery)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<District>()
                .HasMany(e => e.AccountAddresses)
                .WithRequired(e => e.District)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Feedback>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Feedback>()
                .Property(e => e.Stastus)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Genre>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Genre>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Genre)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Oder_Detail>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Oder_Detail>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Oder_Detail)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Payment>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Payment>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Payment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Quantity)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Create_by)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Feedbacks)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => new { e.Product_id, e.Genre_id, e.Disscount_id })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Oder_Detail)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => new { e.Product_id, e.Genre_id, e.Disscount_id })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductImages)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => new { e.Product_id, e.Genre_id, e.Disscount_id });

            modelBuilder.Entity<Province>()
                .HasMany(e => e.AccountAddresses)
                .WithRequired(e => e.Province)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ward>()
                .HasMany(e => e.AccountAddresses)
                .WithRequired(e => e.Ward)
                .WillCascadeOnDelete(false);
        }
    }
}
