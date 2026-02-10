using Microsoft.EntityFrameworkCore;
using SupermarketPOS.Core.Models;

namespace SupermarketPOS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SupermarketPOS",
                "supermarket.db");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.HasIndex(e => e.Barcode).IsUnique();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CostPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryID);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);
            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(e => e.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Inventory configuration
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryID);
            entity.Property(e => e.QuantityOnHand).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ReorderQuantity).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                .WithOne(p => p.Inventory)
                .HasForeignKey<Inventory>(e => e.ProductID);
        });

        // Sale configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleID);
            entity.HasIndex(e => e.SaleNumber).IsUnique();
            entity.HasIndex(e => e.SaleDate);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
        });

        // SaleItem configuration
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.SaleItemID);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(e => e.SaleID);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerID);
            entity.Property(e => e.TotalPurchases).HasColumnType("decimal(18,2)");
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin user (password: admin123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserID = 1,
                Username = "admin",
                PasswordHash = "$2a$11$8K1p/a0dL3LHxRxcwNlnie.WU0/T87qxHtE7r6L5Z/8qKvsoHqP.u", // BCrypt hash
                FullName = "Administrator",
                Role = "Admin",
                IsActive = true
            }
        );

        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryID = 1, Name = "Beverages", SortOrder = 1 },
            new Category { CategoryID = 2, Name = "Dairy", SortOrder = 2 },
            new Category { CategoryID = 3, Name = "Bakery", SortOrder = 3 },
            new Category { CategoryID = 4, Name = "Meat", SortOrder = 4 },
            new Category { CategoryID = 5, Name = "Produce", SortOrder = 5 },
            new Category { CategoryID = 6, Name = "Snacks", SortOrder = 6 }
        );
    }
}
