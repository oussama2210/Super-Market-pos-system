using SupermarketPOS.UI.ViewModels;
using System.Windows.Controls;

namespace SupermarketPOS.UI.Views;

public partial class ProductsView : Page
{
    public ProductsView()
    {
        InitializeComponent();
        DataContext = new ProductsViewModel();
    }
}
