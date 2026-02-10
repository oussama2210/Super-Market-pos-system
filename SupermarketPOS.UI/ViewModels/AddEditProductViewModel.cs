using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupermarketPOS.Core.Models;
using SupermarketPOS.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SupermarketPOS.UI.ViewModels;

public partial class AddEditProductViewModel : ObservableObject
{
    private readonly Action<Product> _onSave;
    private readonly Product? _existingProduct;
    private readonly DataService _dataService;

    [ObservableProperty]
    private string windowTitle = "Add New Product";

    [ObservableProperty]
    private string productName = string.Empty;

    [ObservableProperty]
    private string barcode = string.Empty;

    [ObservableProperty]
    private string shortCode = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private decimal unitPrice;

    [ObservableProperty]
    private decimal costPrice;

    [ObservableProperty]
    private int initialStock;

    [ObservableProperty]
    private bool isActive = true;

    [ObservableProperty]
    private Category? selectedCategory;

    public ObservableCollection<Category> Categories => _dataService.Categories;

    public AddEditProductViewModel(Action<Product> onSave, Product? existingProduct = null)
    {
        _onSave = onSave;
        _existingProduct = existingProduct;
        _dataService = DataService.Instance;

        SelectedCategory = Categories.FirstOrDefault();

        if (existingProduct != null)
        {
            WindowTitle = "Edit Product";
            LoadProductData(existingProduct);
        }
    }

    private void LoadProductData(Product product)
    {
        ProductName = product.Name;
        Barcode = product.Barcode;
        ShortCode = product.ShortCode;
        Description = product.Description ?? string.Empty;
        UnitPrice = product.UnitPrice;
        CostPrice = product.CostPrice;
        IsActive = product.IsActive;
        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
    }

    [RelayCommand]
    private void Save()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            MessageBox.Show("Product name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ShortCode))
        {
            MessageBox.Show("Short code is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ShortCode.Length < 2)
        {
            MessageBox.Show("Short code must be at least 2 characters!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedCategory == null)
        {
            MessageBox.Show("Please select a category!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (UnitPrice <= 0)
        {
            MessageBox.Show("Unit price must be greater than zero!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CostPrice < 0)
        {
            MessageBox.Show("Cost price cannot be negative!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Create or update product
        var product = _existingProduct ?? new Product();
        product.Name = ProductName.Trim();
        product.Barcode = Barcode.Trim();
        product.ShortCode = ShortCode.Trim().ToUpper();
        product.Description = Description.Trim();
        product.CategoryID = SelectedCategory.CategoryID;
        product.Category = SelectedCategory;
        product.UnitPrice = UnitPrice;
        product.CostPrice = CostPrice;
        product.IsActive = IsActive;
        product.ModifiedDate = DateTime.Now;

        if (_existingProduct == null)
        {
            product.CreatedDate = DateTime.Now;
        }

        _onSave(product);

        // Close window
        Application.Current.Windows.OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this)?.Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        Application.Current.Windows.OfType<Window>()
            .FirstOrDefault(w => w.DataContext == this)?.Close();
    }
}
