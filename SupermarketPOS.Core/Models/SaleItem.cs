namespace SupermarketPOS.Core.Models;

public class SaleItem
{
    public int SaleItemID { get; set; }
    public int SaleID { get; set; }
    public int ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }

    // Navigation properties
    public Sale? Sale { get; set; }
    public Product? Product { get; set; }
}
