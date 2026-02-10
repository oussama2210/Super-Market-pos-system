using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SupermarketPOS.UI.Views;

namespace SupermarketPOS.UI;

public partial class MainWindow : Window
{
    private DispatcherTimer? _timer;

    public MainWindow()
    {
        InitializeComponent();
        InitializeTimer();
        NavigationMenu.SelectedIndex = 0;
        
        // Navigate to POS view by default
        ContentFrame.Navigate(new POSView());
    }

    private void InitializeTimer()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy HH:mm:ss");
    }

    private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavigationMenu.SelectedIndex == -1) return;

        switch (NavigationMenu.SelectedIndex)
        {
            case 0:
                StatusText.Text = "POS - Point of Sale";
                ContentFrame.Navigate(new POSView());
                break;
            case 1:
                StatusText.Text = "Products Management";
                ContentFrame.Navigate(new ProductsView());
                break;
            case 2:
                StatusText.Text = "Inventory Management";
                ContentFrame.Navigate(new InventoryView());
                break;
            case 3:
                StatusText.Text = "Reports & Analytics";
                ContentFrame.Navigate(new ReportsView());
                break;
            case 4:
                StatusText.Text = "Customer Management";
                // ContentFrame.Navigate(new CustomersView());
                break;
        }
    }
}