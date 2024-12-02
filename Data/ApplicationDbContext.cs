using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blanca_eManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace Blanca_eManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CompanyInfoModel> Companies { get; set; }
        public DbSet<VATCategory> VATCategories { get; set; }
        public DbSet<RecipeModel> Recipes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurarea entității CategoryModel
            modelBuilder.Entity<CategoryModel>()
                .ToTable("Categories")
                .HasKey(c => c.Id);

            // Configurarea entității ProductModel
            modelBuilder.Entity<ProductModel>()
                .ToTable("Products")
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configurarea pentru proprietatea Price
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); // Precizie: 18, Scală: 2
        }




    }

}
