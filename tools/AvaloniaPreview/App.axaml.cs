using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AIS.UI.Services;
using AIS.UI.ViewModels;

namespace AvaloniaPreview;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = CreateViewModel();
            var window = new MainWindow { DataContext = vm };
            desktop.MainWindow = window;
            if (Program.ShotDir is not null)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
                {
                    await Screenshots.RunAsync(window, vm, Program.ShotDir, Program.ShotScenario!);
                    desktop.Shutdown();
                });
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static VehicleSearchViewModel CreateViewModel()
    {
        var baseUrl = Environment.GetEnvironmentVariable("AIS_API_URL") ?? "http://localhost:5080";
        var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "demo-token");
        return new VehicleSearchViewModel(new VehicleApiClient(http));
    }
}
