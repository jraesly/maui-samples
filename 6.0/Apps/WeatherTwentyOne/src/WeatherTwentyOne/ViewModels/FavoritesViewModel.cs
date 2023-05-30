using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherTwentyOne.Helpers;
using WeatherTwentyOne.Models;

namespace WeatherTwentyOne.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    private ObservableCollection<FavoriteLocation> favorites;
    private static WeatherUpdateService _weatherUpdateService;

    public ObservableCollection<FavoriteLocation> Favorites {
        get {
            return favorites;
        }

        set {
            favorites = value;
            OnPropertyChanged();
        }
    }
    private readonly FavoriteLocationsService _favoriteLocationsService;
    public ICommand LocationSelectedCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }

    public FavoritesViewModel()
    {
        _favoriteLocationsService = new FavoriteLocationsService();
        LoadFavoriteLocations();
        LocationSelectedCommand = new Command<FavoriteLocation>(OnLocationSelected);
    }
    private void LoadFavoriteLocations()
    {
        Favorites = new ObservableCollection<FavoriteLocation>(_favoriteLocationsService.GetFavoriteLocations());
    }

    public void RefreshFavoriteLocations()
    {
        LoadFavoriteLocations();
    }

    private async void OnLocationSelected(FavoriteLocation location)
    {
        if (location == null)
        {
            return;
        }

        // Set LocationService.Instance latitude, longitude, city, and state to locations
        LocationService.Instance.Latitude = location.Latitude ;
        LocationService.Instance.Longitude = location.Longitude;
        LocationService.Instance.City = location.City;
        LocationService.Instance.State = location.State;
        _weatherUpdateService = new WeatherUpdateService();

       await _weatherUpdateService.UpdateWeatherData();
    }



}
