using SupermarketPOS.Core.Models;
using SupermarketPOS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace SupermarketPOS.UI.Services;

public class DataService
{
    private static DataService? _instance;
    private static readonly object _lock = new();
    private readonly ApplicationDbContext _context;

    public static DataService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new DataService();
                }
            }
            return _instance;
        }
    }

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Inventory> InventoryItems { get; } = new();
    public ObservableCollection<Sale> Sales { get; } = new();

    private DataService()
    {
        _context = new ApplicationDbContext();
        InitializeDatabase();
        LoadDataFromDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            _context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }

    private void LoadDataFromDatabase()
    {
        try
        {
            var categories = _context.Categories.ToList();
            if (categories.Count == 0)
            {
                LoadDefaultCategories();
            }
            else
            {
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }

            var products = _context.Products.Include(p => p.Category).ToList();
            
            if (products.Count == 0)
            {
                LoadDefaultProducts();
            }
            else
            {
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }

            var inventory = _context.Inventories.Include(i => i.Product).ThenInclude(p => p!.Category).ToList();
            
            foreach (var item in inventory)
            {
                InventoryItems.Add(item);
            }

            var sales = _context.Sales.Include(s => s.SaleItems).Include(s => s.Customer).OrderByDescending(s => s.SaleDate).ToList();
            
            foreach (var sale in sales)
            {
                Sales.Add(sale);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private void LoadDefaultCategories()
    {
        var defaultCategories = new[]
        {
            new Category { Name = "Beverages", SortOrder = 1 },
            new Category { Name = "Dairy", SortOrder = 2 },
            new Category { Name = "Bakery", SortOrder = 3 },
            new Category { Name = "Meat", SortOrder = 4 },
            new Category { Name = "Produce", SortOrder = 5 },
            new Category { Name = "Snacks", SortOrder = 6 },
            new Category { Name = "Frozen", SortOrder = 7 },
            new Category { Name = "Household", SortOrder = 8 }
        };

        foreach (var category in defaultCategories)
        {
            _context.Categories.Add(category);
            Categories.Add(category);
        }
        _context.SaveChanges();
    }

    private void LoadDefaultProducts()
    {
        var beverageCategory = Categories.FirstOrDefault(c => c.Name == "Beverages");
        var dairyCategory = Categories.FirstOrDefault(c => c.Name == "Dairy");
        var bakeryCategory = Categories.FirstOrDefault(c => c.Name == "Bakery");
        var snacksCategory = Categories.FirstOrDefault(c => c.Name == "Snacks");

        if (beverageCategory == null || dairyCategory == null || bakeryCategory == null || snacksCategory == null)
            return;

        var defaultProducts = new[]
        {
            new { Product = new Product { Barcode = "1234567890123", ShortCode = "COC", Name = "Coca Cola 500ml", CategoryID = beverageCategory.CategoryID, UnitPrice = 1.99m, CostPrice = 1.20m, IsActive = true }, Stock = 150 },
            new { Product = new Product { Barcode = "1234567890124", ShortCode = "MLK", Name = "Fresh Milk 1L", CategoryID = dairyCategory.CategoryID, UnitPrice = 3.49m, CostPrice = 2.50m, IsActive = true }, Stock = 15 },
            new { Product = new Product { Barcode = "1234567890125", ShortCode = "BRD", Name = "White Bread", CategoryID = bakeryCategory.CategoryID, UnitPrice = 2.99m, CostPrice = 1.80m, IsActive = true }, Stock = 0 },
            new { Product = new Product { Barcode = "1234567890126", ShortCode = "OJ", Name = "Orange Juice 1L", CategoryID = beverageCategory.CategoryID, UnitPrice = 4.99m, CostPrice = 3.20m, IsActive = true }, Stock = 45 },
            new { Product = new Product { Barcode = "1234567890127", ShortCode = "CHZ", Name = "Cheddar Cheese 200g", CategoryID = dairyCategory.CategoryID, UnitPrice = 5.99m, CostPrice = 4.00m, IsActive = true }, Stock = 8 },
            new { Product = new Product { Barcode = "1234567890128", ShortCode = "CHP", Name = "Potato Chips 150g", CategoryID = snacksCategory.CategoryID, UnitPrice = 2.49m, CostPrice = 1.50m, IsActive = true }, Stock = 80 },
            new { Product = new Product { Barcode = "1234567890129", ShortCode = "WTR", Name = "Mineral Water 1.5L", CategoryID = beverageCategory.CategoryID, UnitPrice = 1.29m, CostPrice = 0.80m, IsActive = true }, Stock = 200 },
            new { Product = new Product { Barcode = "1234567890130", ShortCode = "YOG", Name = "Yogurt 500g", CategoryID = dairyCategory.CategoryID, UnitPrice = 3.99m, CostPrice = 2.80m, IsActive = true }, Stock = 25 }
        };

        foreach (var item in defaultProducts)
        {
            _context.Products.Add(item.Product);
            _context.SaveChanges();

            item.Product.Category = Categories.FirstOrDefault(c => c.CategoryID == item.Product.CategoryID);
            Products.Add(item.Product);

            var inventory = new Inventory
            {
                ProductID = item.Product.ProductID,
                Product = item.Product,
                QuantityOnHand = item.Stock,
                MinimumLevel = 10,
                MaximumLevel = 100,
                ReorderLevel = 20,
                ReorderQuantity = 50,
                LastRestockDate = DateTime.Now
            };
            _context.Inventories.Add(inventory);
            InventoryItems.Add(inventory);
        }
        _context.SaveChanges();
    }

    public int AddProduct(Product product, int initialStock = 0)
    {
        try
        {
            product.CreatedDate = DateTime.Now;
            product.ModifiedDate = DateTime.Now;
            
            if (product.Category == null && product.CategoryID > 0)
            {
                product.Category = Categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
            }
            
            _context.Products.Add(product);
            _context.SaveChanges();
            
            Products.Add(product);

            if (initialStock >= 0)
            {
                var inventory = new Inventory
                {
                    ProductID = product.ProductID,
                    Product = product,
                    QuantityOnHand = initialStock,
                    MinimumLevel = 10,
                    MaximumLevel = 100,
                    ReorderLevel = 20,
                    ReorderQuantity = 50,
                    LastRestockDate = DateTime.Now
                };
                _context.Inventories.Add(inventory);
                _context.SaveChanges();
                
                InventoryItems.Add(inventory);
            }

            return product.ProductID;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product: {ex.Message}");
            return 0;
        }
    }

    public void UpdateProduct(Product product)
    {
        try
        {
            var existing = _context.Products.Find(product.ProductID);
            if (existing != null)
            {
                product.ModifiedDate = DateTime.Now;
                
                _context.Entry(existing).CurrentValues.SetValues(product);
                _context.SaveChanges();
                
                var index = Products.IndexOf(Products.First(p => p.ProductID == product.ProductID));
                Products[index] = product;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product: {ex.Message}");
        }
    }

    public void DeleteProduct(Product product)
    {
        try
        {
            var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == product.ProductID);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
                InventoryItems.Remove(InventoryItems.First(i => i.ProductID == product.ProductID));
            }
            
            _context.Products.Remove(product);
            _context.SaveChanges();
            
            Products.Remove(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting product: {ex.Message}");
        }
    }

    public Inventory? GetInventory(int productId)
    {
        return InventoryItems.FirstOrDefault(i => i.ProductID == productId);
    }

    public void UpdateInventory(Inventory inventory)
    {
        try
        {
            var existing = _context.Inventories.Find(inventory.InventoryID);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(inventory);
                _context.SaveChanges();
                
                var index = InventoryItems.IndexOf(InventoryItems.First(i => i.InventoryID == inventory.InventoryID));
                InventoryItems[index] = inventory;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating inventory: {ex.Message}");
        }
    }

    public int AddSale(Sale sale)
    {
        try
        {
            sale.SaleDate = DateTime.Now;
            sale.SaleNumber = $"SALE-{DateTime.Now:yyyyMMddHHmmss}";
            
            _context.Sales.Add(sale);
            _context.SaveChanges();
            
            Sales.Insert(0, sale);
            
            return sale.SaleID;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding sale: {ex.Message}");
            return 0;
        }
    }

    public int GetNextProductId() => Products.Any() ? Products.Max(p => p.ProductID) + 1 : 1;
    public int GetNextInventoryId() => InventoryItems.Any() ? InventoryItems.Max(i => i.InventoryID) + 1 : 1;
    public int GetNextSaleId() => Sales.Any() ? Sales.Max(s => s.SaleID) + 1 : 1;
}
