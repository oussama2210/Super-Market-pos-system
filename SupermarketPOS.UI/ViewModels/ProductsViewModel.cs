using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupermarketPOS.Core.Models;
using SupermarketPOS.UI.Services;
using SupermarketPOS.UI.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace SupermarketPOS.UI.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Product? selectedProduct;

    public ObservableCollection<Product> Products => _dataService.Products;
    public ObservableCollection<Product> FilteredProducts { get; } = new();

    public ProductsViewModel()
    {
        _dataService = DataService.Instance;
        RefreshFilteredProducts();
    }

    [RelayCommand]
    private void AddProduct()
    {
        var viewModel = new AddEditProductViewModel(product =>
        {
            _dataService.AddProduct(product);
            RefreshFilteredProducts();
            MessageBox.Show($"Product '{product.Name}' added successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        });

        var window = new AddEditProductWindow(viewModel);
        window.ShowDialog();
    }

    [RelayCommand]
    private void EditProduct(Product? product)
    {
        if (product == null) return;

        var viewModel = new AddEditProductViewModel(updatedProduct =>
        {
            _dataService.UpdateProduct(updatedProduct);
            RefreshFilteredProducts();
            MessageBox.Show($"Product '{updatedProduct.Name}' updated successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }, product);

        var window = new AddEditProductWindow(viewModel);
        window.ShowDialog();
    }

    [RelayCommand]
    private void DeleteProduct(Product? product)
    {
        if (product == null) return;

        var result = MessageBox.Show($"Are you sure you want to delete '{product.Name}'?", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _dataService.DeleteProduct(product);
            RefreshFilteredProducts();
            MessageBox.Show("Product deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilteredProducts();
    }

    [RelayCommand]
    private void Search()
    {
        RefreshFilteredProducts();
    }

    private void RefreshFilteredProducts()
    {
        FilteredProducts.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? Products
            : Products.Where(p =>
                p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.Barcode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.ShortCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var product in filtered)
        {
            FilteredProducts.Add(product);
        }
    }
}
