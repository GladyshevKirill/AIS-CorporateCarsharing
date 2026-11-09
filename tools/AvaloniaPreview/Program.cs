using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;
using AIS.UI.ViewModels;

namespace AvaloniaPreview;

public static class Program
{
    public static string? ShotDir;
    public static string? ShotScenario;

    public static async Task<int> Main(string[] args)
    {
        // Режим снимков экрана: --screenshots <каталог> <ok|error>
        if (args.Length >= 3 && args[0] == "--screenshots")
        {
            ShotDir = args[1];
            ShotScenario = args[2];
            return AppBuilder.Configure<App>().UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
                .StartWithClassicDesktopLifetime(Array.Empty<string>());
        }

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UsePlatformDetect();
    }
}

public static class Screenshots
{
    public static async Task RunAsync(MainWindow window, VehicleSearchViewModel vm, string directory, string scenario)
    {
        Directory.CreateDirectory(directory);
        await Task.Delay(300);
        if (scenario == "ok")
        {
            Capture(window, Path.Combine(directory, "01_start.png"));
            await Search(vm);
            Capture(window, Path.Combine(directory, "02_all.png"));
            vm.SelectedClass = "BUSINESS";
            await Search(vm);
            Capture(window, Path.Combine(directory, "03_business.png"));
            vm.SelectedClass = "VAN";
            await Search(vm);
            Capture(window, Path.Combine(directory, "04_van.png"));
        }
        else
        {
            await Search(vm);
            Capture(window, Path.Combine(directory, "05_error.png"));
        }
    }

    private static async Task Search(VehicleSearchViewModel vm)
    {
        vm.SearchCommand.Execute(null);
        await Task.Delay(1500);
    }

    private static void Capture(MainWindow window, string path)
    {
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        using var frame = window.CaptureRenderedFrame();
        frame?.Save(path);
        Console.WriteLine("saved " + path);
    }
}
