namespace SupermarketPOS.Core.Models;

public class Sale
{
    public int SaleID { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int? CustomerID { get; set; }
    public int UserID { get; set; }
    public bool IsPrinted { get; set; }
    public bool IsVoided { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public User? User { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
