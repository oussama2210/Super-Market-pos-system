using SupermarketPOS.UI.ViewModels;
using System.Windows;

namespace SupermarketPOS.UI.Views;

public partial class AddEditProductWindow : Window
{
    public AddEditProductWindow(AddEditProductViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
