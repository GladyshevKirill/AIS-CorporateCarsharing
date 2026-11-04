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

    public VehicleSearchViewModel(IVehicleApiClient api)
    {
        _api = api;
        SearchCommand = new AsyncRelayCommand(SearchAsync, () => From < To);
    }

    public ObservableCollection<VehicleItemViewModel> Vehicles { get; } = new();

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
            var page = await _api.SearchAsync(From.ToUniversalTime(), To.ToUniversalTime(), null, 1, 50);
            Vehicles.Clear();
            foreach (var vehicle in page.Items)
            {
                Vehicles.Add(new VehicleItemViewModel(vehicle));
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = "Не удалось получить список автомобилей: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
