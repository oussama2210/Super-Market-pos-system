using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupermarketPOS.Core.Models;
using SupermarketPOS.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SupermarketPOS.UI.ViewModels;

public partial class InventoryViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Inventory? selectedItem;

    public ObservableCollection<Inventory> InventoryItems => _dataService.InventoryItems;
    public ObservableCollection<Inventory> FilteredItems { get; } = new();

    public InventoryViewModel()
    {
        _dataService = DataService.Instance;
        RefreshFilteredItems();
    }

    [RelayCommand]
    private void AdjustStock(Inventory? item)
    {
        if (item == null)
        {
            MessageBox.Show("Please select an item to adjust stock", "Adjust Stock",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var input = Microsoft.VisualBasic.Interaction.InputBox(
            $"Current stock: {item.QuantityOnHand}\n\nEnter adjustment amount (use + or - for add/remove):",
            "Adjust Stock",
            "0");

        if (string.IsNullOrWhiteSpace(input)) return;

        if (int.TryParse(input, out int adjustment))
        {
            item.QuantityOnHand += adjustment;
            item.LastRestockDate = DateTime.Now;

            MessageBox.Show($"Stock adjusted successfully!\nNew quantity: {item.QuantityOnHand}",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            RefreshFilteredItems();
        }
        else
        {
            MessageBox.Show("Invalid number format!", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshFilteredItems();
        MessageBox.Show("Inventory refreshed!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilteredItems();
    }

    private void RefreshFilteredItems()
    {
        FilteredItems.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? InventoryItems
            : InventoryItems.Where(i =>
                i.Product != null && (
                    i.Product.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Product.ShortCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

        foreach (var item in filtered)
        {
            FilteredItems.Add(item);
        }
    }
}
