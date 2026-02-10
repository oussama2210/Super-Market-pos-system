namespace SupermarketPOS.Core.Models;

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryID { get; set; }
    public string? IconName { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
