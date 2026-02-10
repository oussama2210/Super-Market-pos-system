# 🛒 Supermarket POS System

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) ![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) ![WPF](https://img.shields.io/badge/WPF-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white) ![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white) ![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)

A professional, robust, and modern Point of Sale (POS) system designed for supermarkets and retail stores. Built with **.NET 8** and **WPF**, following the **MVVM** architecture pattern for maintainability and scalability.

## ✨ Features

- **Product Management**:
  - Add, edit, and delete products easily.
  - Support for barcodes, short codes, and categories.
  - Image support for products.
- **Inventory Control**:
  - Real-time stock tracking.
  - Automatic inventory deduction upon sales.
  - Low stock alerts (visual indicators).
- **Sales Processing**:
  - Efficient point-of-sale interface.
  - Barcode scanning support.
  - Automatic tax and discount calculations.
  - Multiple payment methods.
- **Reporting & Analytics**:
  - View sales history.
  - Track revenue and performance.
- **Data Persistence**:
  - **SQLite** database ensures data is saved locally and reliably.
  - Automatic database creation and seeding on first run.
  - Data survives application restarts (Products, Inventory, Sales).

## 🛠️ Tech Stack

- **Framework**: .NET 8.0
- **UI Technology**: WPF (Windows Presentation Foundation)
- **Language**: C#
- **Database**: SQLite (Entity Framework Core or direct interaction)
- **Architecture**: MVVM (Model-View-ViewModel)

## 🚀 Getting Started

### Prerequisites

- Windows 10 or 11.
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

### Installation

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/yourusername/SupermarketPOS.git
    cd SupermarketPOS
    ```

2.  **Build and Run**:
    Simply double-click the `run.bat` file in the root directory.

    *Alternatively, via terminal:*
    ```bash
    run.bat
    ```

    This script will:
    - Build the solution using `dotnet build`.
    - Launch the application automatically.

## 📂 Project Structure

- **SupermarketPOS.Core**: Core business logic and models.
- **SupermarketPOS.Data**: Database context and data access layer.
- **SupermarketPOS.Reports**: Logic for generating reports and analytics.
- **SupermarketPOS.UI**: The main WPF application (Views, ViewModels, Resources).

## 🗄️ Database

The application uses a local SQLite database located at:
`C:\Users\[YourUsername]\AppData\Roaming\SupermarketPOS\supermarket.db`

You can use any SQLite browser (like [DB Browser for SQLite](https://sqlitebrowser.org/)) to inspect the data manually if needed.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the project.
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
