namespace SupermarketPOS.Core.Models;

public class Product
{
    public int ProductID { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty; // Custom code for products without barcode (e.g., "COC" for Coca Cola)
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryID { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    // Navigation properties
    public Category? Category { get; set; }
    public Inventory? Inventory { get; set; }
}
