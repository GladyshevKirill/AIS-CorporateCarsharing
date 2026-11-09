using System.Collections.ObjectModel;
using System.Net.Http;
using AIS.UI.Services;

namespace AIS.UI.ViewModels;

/// <summary>ViewModel экрана поиска свободных автомобилей.</summary>
public class VehicleSearchViewModel : ViewModelBase
{
    private readonly IVehicleApiClient _api;
    private DateTime _from = DateTime.Today.AddDays(1).AddHours(9);
    private DateTime _to = DateTime.Today.AddDays(1).AddHours(18);
    private bool _isBusy;
    private string? _errorMessage;
    private string _selectedClass = AnyClass;
    private string _resultText = "Задайте интервал и нажмите «Найти»";

    /// <summary>Значение фильтра «любой класс».</summary>
    public const string AnyClass = "Любой";

    public VehicleSearchViewModel(IVehicleApiClient api)
    {
        _api = api;
        SearchCommand = new AsyncRelayCommand(SearchAsync, () => From < To);
    }

    public ObservableCollection<VehicleItemViewModel> Vehicles { get; } = new();

    public IReadOnlyList<string> Classes { get; } = new[] { AnyClass, "ECONOMY", "BUSINESS", "VAN" };

    public string SelectedClass
    {
        get => _selectedClass;
        set => SetField(ref _selectedClass, value);
    }

    public string ResultText
    {
        get => _resultText;
        private set => SetField(ref _resultText, value);
    }

    public DateTime From
    {
        get => _from;
        set
        {
            if (SetField(ref _from, value))
            {
                SearchCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public DateTime To
    {
        get => _to;
        set
        {
            if (SetField(ref _to, value))
            {
                SearchCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public AsyncRelayCommand SearchCommand { get; }

    public async Task SearchAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var vehicleClass = SelectedClass == AnyClass ? null : SelectedClass;
            var page = await _api.SearchAsync(From.ToUniversalTime(), To.ToUniversalTime(), vehicleClass, 1, 50);
            Vehicles.Clear();
            foreach (var vehicle in page.Items)
            {
                Vehicles.Add(new VehicleItemViewModel(vehicle));
            }

            ResultText = page.Total == 0
                ? "Свободных автомобилей на этот интервал нет"
                : $"Найдено автомобилей: {page.Total}";
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            Vehicles.Clear();
            ResultText = string.Empty;
            ErrorMessage = "Не удалось получить список автомобилей: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
