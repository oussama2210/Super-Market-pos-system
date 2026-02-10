namespace SupermarketPOS.Core.Models;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Cashier"; // Admin, Cashier, Manager
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }

    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
