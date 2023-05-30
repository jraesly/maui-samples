using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherTwentyOne.Helpers;
using WeatherTwentyOne.Models;
using WeatherTwentyOne.Pages;
using WeatherTwentyOne.Views;
using Microsoft.Maui.Dispatching;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Maui.Devices.Sensors;

namespace WeatherTwentyOne.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly HomePage _homePage;

        private static WeatherUpdateService _weatherUpdateService;
        public static WeatherUpdateService WeatherUpdateServiceInstance
        {
            get => _weatherUpdateService;
            set => _weatherUpdateService ??= value;
        }
        private string _favoriteImageSource;
        public string FavoriteImageSource
        {
            get => _favoriteImageSource;
            set => SetProperty(ref _favoriteImageSource, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private OpenWeatherMapModel _weatherData;
        public OpenWeatherMapModel WeatherData
        {
            get => _weatherData;
            set => SetProperty(ref _weatherData, value);
        }

        private string _locationTitle = "";
        public string LocationTitle
        {
            get => _locationTitle;
            set => SetProperty(ref _locationTitle, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand QuitCommand { get; }
        public ICommand AddLocationCommand { get; }
        public ICommand ChangeLocationCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleModeCommand { get; }
        public ICommand FavoritesCommand { get; }

        public HomeViewModel(HomePage homePage)
        {
            _homePage = homePage;

            QuitCommand = new Command(Application.Current.Quit);
            ChangeLocationCommand = new Command<string>(ChangeLocation);
            RefreshCommand = new Command(async () => await RefreshWeatherData());
            ToggleModeCommand = new Command(ToggleMode);
            FavoritesCommand = new Command(Favorites);
        }
        private void Favorites()
        {
            var favLocation = new FavoriteLocation
            {
                Latitude = LocationService.Instance.Latitude,
                Longitude = LocationService.Instance.Longitude,
                City = LocationService.Instance.City,
                State = LocationService.Instance.State
            };
            var favlocService = new FavoriteLocationsService();
            if (favlocService.LocationExists(favLocation))
            {
                favlocService.RemoveFavoriteLocation(favLocation);
            }
            else
            {
                favlocService.AddFavoriteLocation(favLocation);
            }
            IsLocationAFavorite(favlocService, favLocation);
        }
        private async void ChangeLocation(string location)
        {
            // change primary location
            string cityState = await _homePage.DisplayPromptAsync("Enter City, State", "Please enter the City, State name:");
            var newLocation = await LocationHelper.GetLocationFromUserInputAsync(cityState);
            LocationService.Instance.SetLocation(newLocation.latitude, newLocation.longitude, newLocation.city, newLocation.state);
            // Update the LocationTitle property
            LocationTitle = $"{newLocation.city}, {newLocation.state}";
            await RefreshWeatherData();
        }

        private void ToggleMode()
        {
            App.Current.UserAppTheme = App.Current.UserAppTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        }
        public static async Task<HomeViewModel> CreateAsync(HomePage homePage)
        {
            var viewModel = new HomeViewModel(homePage);
            await viewModel.InitializeAsync();
            return viewModel;
        }
        private async Task InitializeAsync()
        {
            var favlocService = new FavoriteLocationsService();
            await LocationHelper.GetLocationAsync();
            LocationTitle = $"{LocationService.Instance.City}, {LocationService.Instance.State}";
            _weatherUpdateService = new WeatherUpdateService();
            _weatherUpdateService.WeatherDataUpdated += OnWeatherDataUpdated;
            _weatherUpdateService.Start();
            IsLocationAFavorite(favlocService);
        }

        public void IsLocationAFavorite(FavoriteLocationsService favlocService, FavoriteLocation favoriteLocation = null)
        {
            favoriteLocation ??= new FavoriteLocation
                {
                    Latitude = LocationService.Instance.Latitude,
                    Longitude = LocationService.Instance.Longitude,
                    City = LocationService.Instance.City,
                    State = LocationService.Instance.State
                };

            if (favlocService.LocationExists(favoriteLocation))
            {
                FavoriteImageSource = "heart.png";
            }
            else
            {
                FavoriteImageSource = "unheart.png";
            }
        }

        private void OnWeatherDataUpdated(object sender, OpenWeatherMapModel weatherData)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                WeatherData = weatherData;
            });
            _homePage.InitializeWidgets(weatherData);
        }

        private async Task RefreshWeatherData()
        {
            try
            {
                // Add IsRefreshing = true and task.run
                IsRefreshing = true;
                await Task.Run(async () =>
                {
                    await _weatherUpdateService.UpdateWeatherData();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating weather data: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
