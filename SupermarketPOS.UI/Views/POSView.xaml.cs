using SupermarketPOS.UI.ViewModels;
using System.Windows.Controls;

namespace SupermarketPOS.UI.Views;

public partial class POSView : Page
{
    public POSView()
    {
        InitializeComponent();
        DataContext = new POSViewModel();
    }
}
