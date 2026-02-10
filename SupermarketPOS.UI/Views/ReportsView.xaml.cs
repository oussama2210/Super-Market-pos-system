using SupermarketPOS.UI.ViewModels;
using System.Windows.Controls;

namespace SupermarketPOS.UI.Views;

public partial class ReportsView : Page
{
    public ReportsView()
    {
        InitializeComponent();
        DataContext = new ReportsViewModel();
    }
}
