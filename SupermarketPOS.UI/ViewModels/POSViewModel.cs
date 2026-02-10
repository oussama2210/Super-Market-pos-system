using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupermarketPOS.Core.Models;
using SupermarketPOS.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SupermarketPOS.UI.ViewModels;

public partial class POSViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private decimal subTotal;

    [ObservableProperty]
    private decimal taxAmount;

    [ObservableProperty]
    private decimal discountAmount;

    [ObservableProperty]
    private decimal totalAmount;

    [ObservableProperty]
    private decimal cashReceived;

    [ObservableProperty]
    private decimal changeAmount;

    public ObservableCollection<CartItem> CartItems { get; } = new();
    public ObservableCollection<Category> Categories => _dataService.Categories;
    public ObservableCollection<Product> Products => _dataService.Products;
    public ObservableCollection<Product> FilteredProducts { get; } = new();

    public POSViewModel()
    {
        _dataService = DataService.Instance;
        RefreshFilteredProducts();
        CalculateTotals();
    }

    private void RefreshFilteredProducts()
    {
        FilteredProducts.Clear();
        foreach (var product in Products.Where(p => p.IsActive))
        {
            FilteredProducts.Add(product);
        }
    }

    [RelayCommand]
    private void AddToCart(Product? product)
    {
        if (product == null) return;

        var existingItem = CartItems.FirstOrDefault(x => x.ProductID == product.ProductID);
        if (existingItem != null)
        {
            existingItem.Quantity++;
            existingItem.UpdateLineTotal();
        }
        else
        {
            CartItems.Add(new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.Name,
                UnitPrice = product.UnitPrice,
                Quantity = 1
            });
        }

        CalculateTotals();
        SearchText = string.Empty; // Clear search after adding
    }

    [RelayCommand]
    private void RemoveFromCart(CartItem? item)
    {
        if (item != null)
        {
            CartItems.Remove(item);
            CalculateTotals();
        }
    }

    [RelayCommand]
    private void IncreaseQuantity(CartItem? item)
    {
        if (item != null)
        {
            item.Quantity++;
            item.UpdateLineTotal();
            CalculateTotals();
        }
    }

    [RelayCommand]
    private void DecreaseQuantity(CartItem? item)
    {
        if (item != null && item.Quantity > 1)
        {
            item.Quantity--;
            item.UpdateLineTotal();
            CalculateTotals();
        }
    }

    [RelayCommand]
    private void ClearCart()
    {
        CartItems.Clear();
        CalculateTotals();
        CashReceived = 0;
        ChangeAmount = 0;
    }

    [RelayCommand]
    private void ProcessPayment()
    {
        if (CartItems.Count == 0)
        {
            MessageBox.Show("Cart is empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CashReceived < TotalAmount)
        {
            MessageBox.Show("Insufficient payment!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ChangeAmount = CashReceived - TotalAmount;

        // Create sale record
        var sale = new Sale
        {
            SaleDate = DateTime.Now,
            UserID = 1, // Default user
            CustomerID = null, // Walk-in customer
            SubTotal = SubTotal,
            TaxAmount = TaxAmount,
            DiscountAmount = DiscountAmount,
            TotalAmount = TotalAmount,
            PaymentMethod = "Cash",
            IsPrinted = false,
            IsVoided = false
        };

        // Add sale items
        sale.SaleItems = new List<SaleItem>();
        foreach (var cartItem in CartItems)
        {
            sale.SaleItems.Add(new SaleItem
            {
                ProductID = cartItem.ProductID,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                LineTotal = cartItem.LineTotal
            });

            // Update inventory
            var inventory = _dataService.GetInventory(cartItem.ProductID);
            if (inventory != null)
            {
                inventory.QuantityOnHand -= cartItem.Quantity;
                _dataService.UpdateInventory(inventory);
            }
        }

        // Save sale to data service
        _dataService.AddSale(sale);
        
        MessageBox.Show($"Payment successful!\n\nTotal: {TotalAmount:C2}\nCash: {CashReceived:C2}\nChange: {ChangeAmount:C2}", 
            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        // Clear cart after successful payment
        ClearCart();
    }

    [RelayCommand]
    private void ProcessExactPayment()
    {
        if (CartItems.Count == 0)
        {
            MessageBox.Show("Cart is empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Set cash received to exact amount
        CashReceived = TotalAmount;
        ChangeAmount = 0;

        // Create sale record
        var sale = new Sale
        {
            SaleDate = DateTime.Now,
            UserID = 1, // Default user
            CustomerID = null, // Walk-in customer
            SubTotal = SubTotal,
            TaxAmount = TaxAmount,
            DiscountAmount = DiscountAmount,
            TotalAmount = TotalAmount,
            PaymentMethod = "Card",
            IsPrinted = false,
            IsVoided = false
        };

        // Add sale items
        sale.SaleItems = new List<SaleItem>();
        foreach (var cartItem in CartItems)
        {
            sale.SaleItems.Add(new SaleItem
            {
                ProductID = cartItem.ProductID,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                LineTotal = cartItem.LineTotal
            });

            // Update inventory
            var inventory = _dataService.GetInventory(cartItem.ProductID);
            if (inventory != null)
            {
                inventory.QuantityOnHand -= cartItem.Quantity;
                _dataService.UpdateInventory(inventory);
            }
        }

        // Save sale to data service
        _dataService.AddSale(sale);
        
        MessageBox.Show($"Payment successful!\n\nTotal: {TotalAmount:C2}\nPayment Method: Card/Exact", 
            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        // Clear cart after successful payment
        ClearCart();
    }

    partial void OnCashReceivedChanged(decimal value)
    {
        ChangeAmount = value - TotalAmount;
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterProducts();
    }

    private void FilterProducts()
    {
        FilteredProducts.Clear();

        var activeProducts = Products.Where(p => p.IsActive);

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var product in activeProducts)
            {
                FilteredProducts.Add(product);
            }
            return;
        }

        var searchUpper = SearchText.ToUpper();
        var searchLower = SearchText.ToLower();
        
        var filtered = activeProducts.Where(p =>
            p.Name.ToLower().Contains(searchLower) ||
            p.Barcode.Contains(SearchText) ||
            p.ShortCode.ToUpper().Contains(searchUpper)
        );

        foreach (var product in filtered)
        {
            FilteredProducts.Add(product);
        }

        // If exact barcode or short code match, auto-add to cart
        var exactMatch = activeProducts.FirstOrDefault(p => 
            p.Barcode == SearchText || 
            p.ShortCode.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
            
        if (exactMatch != null)
        {
            AddToCart(exactMatch);
        }
    }

    [RelayCommand]
    private void SearchProduct()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            MessageBox.Show("Please enter a product name, barcode, or short code", "Search", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var activeProducts = Products.Where(p => p.IsActive);

        // Search by barcode first
        var product = activeProducts.FirstOrDefault(p => p.Barcode == SearchText);
        
        // If not found by barcode, search by short code
        if (product == null)
        {
            product = activeProducts.FirstOrDefault(p => 
                p.ShortCode.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        
        // If still not found, search by name
        if (product == null)
        {
            product = activeProducts.FirstOrDefault(p => 
                p.Name.ToLower().Contains(SearchText.ToLower()));
        }

        if (product != null)
        {
            AddToCart(product);
        }
        else
        {
            MessageBox.Show($"Product not found: {SearchText}", "Search", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void FilterByCategory(Category? category)
    {
        FilteredProducts.Clear();
        
        var activeProducts = Products.Where(p => p.IsActive);
        
        if (category == null)
        {
            // Show all active products
            foreach (var product in activeProducts)
            {
                FilteredProducts.Add(product);
            }
        }
        else
        {
            // Filter by category
            var filtered = activeProducts.Where(p => p.CategoryID == category.CategoryID);
            
            foreach (var product in filtered)
            {
                FilteredProducts.Add(product);
            }
        }
        
        SearchText = string.Empty; // Clear search when filtering by category
    }

    private void CalculateTotals()
    {
        SubTotal = CartItems.Sum(x => x.LineTotal);
        TaxAmount = SubTotal * 0.10m; // 10% tax
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
        
        if (CashReceived > 0)
        {
            ChangeAmount = CashReceived - TotalAmount;
        }
    }
}

public partial class CartItem : ObservableObject
{
    public int ProductID { get; set; }

    [ObservableProperty]
    private string productName = string.Empty;

    [ObservableProperty]
    private decimal unitPrice;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private decimal lineTotal;

    public void UpdateLineTotal()
    {
        LineTotal = UnitPrice * Quantity;
    }

    partial void OnQuantityChanged(int value)
    {
        UpdateLineTotal();
    }

    partial void OnUnitPriceChanged(decimal value)
    {
        UpdateLineTotal();
    }
}
