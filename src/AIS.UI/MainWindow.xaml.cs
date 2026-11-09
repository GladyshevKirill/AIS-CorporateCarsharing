using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using AIS.UI.Services;
using AIS.UI.ViewModels;

namespace AIS.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Адрес шлюза берётся из переменной окружения AIS_API_URL (по умолчанию локальный демо-сервер).
        var baseUrl = Environment.GetEnvironmentVariable("AIS_API_URL") ?? "http://localhost:5080";
        var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "demo-token");
        SearchView.DataContext = new VehicleSearchViewModel(new VehicleApiClient(http));
    }
}
