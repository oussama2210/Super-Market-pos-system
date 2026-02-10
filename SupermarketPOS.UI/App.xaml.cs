using System.Configuration;
using System.Data;
using System.Windows;

namespace SupermarketPOS.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Handle unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            MessageBox.Show($"Unhandled exception: {args.ExceptionObject}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };
        
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"Dispatcher exception: {args.Exception.Message}\n\n{args.Exception.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
    }
}

