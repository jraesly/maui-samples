using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using WeatherTwentyOne.Models;
using WeatherTwentyOne.Services;
using WeatherTwentyOne.ViewModels;
using Application = Microsoft.Maui.Controls.Application;
using WindowsConfiguration = Microsoft.Maui.Controls.PlatformConfiguration.Windows;

namespace WeatherTwentyOne.Pages;

public partial class HomePage : ContentPage
{
    static bool isSetup = false;
    RestService _restService;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;

        if (!isSetup)
        {
            isSetup = true;

            SetupAppActions();
            SetupTrayIcon();
            _restService = new RestService();
        }
    }

    private void SetupAppActions()
    {
        try
        {
#if WINDOWS
            //AppActions.IconDirectory = Application.Current.On<WindowsConfiguration>().GetImageDirectory();
#endif
            AppActions.SetAsync(
                new AppAction("current_info", "Check Current Weather", icon: "current_info"),
                new AppAction("add_location", "Add a Location", icon: "add_location")
            );
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine("App Actions not supported", ex);
        }
    }

    private void SetupTrayIcon()
    {
        var trayService = ServiceProvider.GetService<ITrayService>();

        if (trayService != null)
        {
            trayService.Initialize();
            trayService.ClickHandler = () =>
                ServiceProvider.GetService<INotificationService>()
                    ?.ShowNotification("Hello Build! 😻 From .NET MAUI", "How's your weather?  It's sunny where we are 🌞");
        }
    }
    async void OnGetWeatherButtonClicked(object sender, EventArgs e)
    {
       // if (!string.IsNullOrWhiteSpace(_cityEntry.Text))
       // {
            OpenWeatherMapModel weatherData = await
            _restService.GetWeatherData(GenerateRequestURL(Constants.OpenWeatherMapEndpoint));

            BindingContext = weatherData;
       // }
    }

    string GenerateRequestURL(string endPoint)
    {
        // Update to get the current city being used
        string requestUri = endPoint;
        requestUri += $"lat=39.616512";
        requestUri += $"&lon=-104.9264128";
        requestUri += $"&APPID={Constants.OpenWeatherMapAPIKey}";
        requestUri += $"&units=imperial";
        return requestUri;
    }
}

