using SupermarketPOS.UI.ViewModels;
using System.Windows.Controls;

namespace SupermarketPOS.UI.Views;

public partial class InventoryView : Page
{
    public InventoryView()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel();
    }
}
