using System.Net.Http;
using System.Windows;
using AIS.UI.Services;
using AIS.UI.ViewModels;

namespace AIS.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var http = new HttpClient { BaseAddress = new Uri("https://gateway.carsharing.example") };
        SearchView.DataContext = new VehicleSearchViewModel(new VehicleApiClient(http));
    }
}
