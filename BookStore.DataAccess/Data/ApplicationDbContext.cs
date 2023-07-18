using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookStore.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext()
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.SeedRoles(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   Id = 11,
                   Title = "Fortune of Time",
                   Author = "Billy Spark",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "SWD9999001",
                   ListPrice = 99,
                   Price = 90,
                   Price50 = 85,
                   Price100 = 80,
                   CategoryId = 2,
                   ImageUrl = ""
               },
               new Product
               {
                   Id = 12,
                   Title = "Dark Skies",
                   Author = "Nancy Hoover",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "CAW777777701",
                   ListPrice = 40,
                   Price = 30,
                   Price50 = 25,
                   Price100 = 20,
                   CategoryId = 2,
                   ImageUrl = ""
               },
               new Product
               {
                   Id = 13,
                   Title = "Vanish in the Sunset",
                   Author = "Julian Button",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "RITO5555501",
                   ListPrice = 55,
                   Price = 50,
                   Price50 = 40,
                   Price100 = 35,
                   CategoryId = 2,
                   ImageUrl = ""
               },
               new Product
               {
                   Id = 14,
                   Title = "Cotton Candy",
                   Author = "Abby Muscles",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "WS3333333301",
                   ListPrice = 70,
                   Price = 65,
                   Price50 = 60,
                   Price100 = 55,
                   CategoryId = 2,
                   ImageUrl = ""
               },
               new Product
               {
                   Id = 15,
                   Title = "Rock in the Ocean",
                   Author = "Ron Parker",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "SOTJ1111111101",
                   ListPrice = 30,
                   Price = 27,
                   Price50 = 25,
                   Price100 = 20,
                   CategoryId = 2,
                   ImageUrl = ""
               },
               new Product
               {
                   Id = 16,
                   Title = "Leaves and Wonders",
                   Author = "Laura Phantom",
                   Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                   ISBN = "FOT000000001",
                   ListPrice = 25,
                   Price = 23,
                   Price50 = 22,
                   Price100 = 20,
                   CategoryId = 2,
                   ImageUrl = ""
               }
               );
        }
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
                new IdentityRole() { Name = "Customer", ConcurrencyStamp = "3", NormalizedName = "Customer" }
                );
        }
    }
}
