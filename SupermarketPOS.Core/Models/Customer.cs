namespace SupermarketPOS.Core.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int LoyaltyPoints { get; set; }
    public decimal TotalPurchases { get; set; }

    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
