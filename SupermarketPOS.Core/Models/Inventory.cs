namespace SupermarketPOS.Core.Models;

public class Inventory
{
    public int InventoryID { get; set; }
    public int ProductID { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal MinimumLevel { get; set; }
    public decimal MaximumLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal ReorderQuantity { get; set; }
    public DateTime? LastRestockDate { get; set; }

    // Computed properties
    public bool IsLowStock => QuantityOnHand <= ReorderLevel;
    public bool IsOutOfStock => QuantityOnHand == 0;

    // Navigation properties
    public Product? Product { get; set; }
}
