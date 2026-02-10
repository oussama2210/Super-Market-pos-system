using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupermarketPOS.Core.Models;
using SupermarketPOS.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SupermarketPOS.UI.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private decimal totalSales;

    [ObservableProperty]
    private int totalTransactions;

    [ObservableProperty]
    private decimal averageSale;

    [ObservableProperty]
    private decimal totalProfit;

    public ObservableCollection<SaleReportItem> SalesData { get; } = new();

    public ReportsViewModel()
    {
        _dataService = DataService.Instance;
        GenerateReport();
    }

    [RelayCommand]
    private void GenerateReport()
    {
        LoadSalesData();
        CalculateSummary();
    }

    private void LoadSalesData()
    {
        SalesData.Clear();

        // Filter sales by date range
        var filteredSales = _dataService.Sales
            .Where(s => s.SaleDate >= StartDate && s.SaleDate <= EndDate.AddDays(1))
            .OrderByDescending(s => s.SaleDate);

        foreach (var sale in filteredSales)
        {
            var itemCount = (int)(sale.SaleItems?.Sum(si => si.Quantity) ?? 0);
            
            SalesData.Add(new SaleReportItem
            {
                SaleID = sale.SaleID,
                SaleDate = sale.SaleDate,
                Customer = sale.Customer ?? new Customer { Name = "Walk-in Customer" },
                ItemCount = itemCount,
                SubTotal = sale.SubTotal,
                TaxAmount = sale.TaxAmount,
                DiscountAmount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                PaymentMethod = sale.PaymentMethod
            });
        }
    }

    private void CalculateSummary()
    {
        TotalSales = SalesData.Sum(s => s.TotalAmount);
        TotalTransactions = SalesData.Count;
        AverageSale = TotalTransactions > 0 ? TotalSales / TotalTransactions : 0;
        
        // Assuming 30% profit margin for demo
        TotalProfit = SalesData.Sum(s => s.SubTotal * 0.30m);
    }

    [RelayCommand]
    private void ExportReport()
    {
        MessageBox.Show($"Exporting report from {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n\n" +
                       $"Total Sales: {TotalSales:C2}\n" +
                       $"Transactions: {TotalTransactions}\n" +
                       $"Average Sale: {AverageSale:C2}\n" +
                       $"Total Profit: {TotalProfit:C2}",
                       "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    partial void OnStartDateChanged(DateTime value)
    {
        GenerateReport();
    }

    partial void OnEndDateChanged(DateTime value)
    {
        GenerateReport();
    }
}

public class SaleReportItem
{
    public int SaleID { get; set; }
    public DateTime SaleDate { get; set; }
    public Customer Customer { get; set; } = new();
    public int ItemCount { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
