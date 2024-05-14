using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Project_KFC_WEB.Models
{
    public partial class KFC_Data : DbContext
    {
        public KFC_Data()
            : base("name=KFC_Data")
        {
        }

        public virtual DbSet<account> accounts { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<discountCode> discountCodes { get; set; }
        public virtual DbSet<food> foods { get; set; }
        public virtual DbSet<foodCategory> foodCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<account>()
                .Property(e => e.userName)
                .IsUnicode(false);

            modelBuilder.Entity<account>()
                .Property(e => e.passWord)
                .IsUnicode(false);

            modelBuilder.Entity<account>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<cart>()
                .Property(e => e.userName)
                .IsUnicode(false);

            modelBuilder.Entity<discountCode>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<food>()
                .HasMany(e => e.carts)
                .WithOptional(e => e.food)
                .HasForeignKey(e => e.idFood);

            modelBuilder.Entity<foodCategory>()
                .HasMany(e => e.foods)
                .WithOptional(e => e.foodCategory)
                .HasForeignKey(e => e.idCategory);
        }
    }
}
