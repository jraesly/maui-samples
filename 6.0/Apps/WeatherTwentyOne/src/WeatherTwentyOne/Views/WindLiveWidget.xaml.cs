using WeatherTwentyOne.Models;
using WeatherTwentyOne.ViewModels;


namespace WeatherTwentyOne.Views;

public partial class WindLiveWidget:VerticalStackLayout
{
    System.Timers.Timer aTimer;
    public OpenWeatherMapModel weatherData;

    public WindLiveWidget(OpenWeatherMapModel weatherData)
    {
        InitializeComponent();
        BindingContext = weatherData;
        if (aTimer == null && DeviceInfo.Platform != DevicePlatform.Android)
            Start();

    }

    public void OnTapped(object sender, EventArgs e)
    {

    }

    public void Start()
    {
        weatherData = BindingContext as OpenWeatherMapModel;

        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(31000);
        // Hook up the Elapsed event for the timer. 
        this.Dispatcher.Dispatch(() =>
        {
            aTimer.Elapsed += UpdateLiveWind;
        });
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    public void Stop()
    {
        aTimer.Enabled = false;
    }

    void UpdateLiveWind(object source, System.Timers.ElapsedEventArgs e)
    {
        var direction = GetDirection();

        this.Dispatcher.Dispatch(() => {
            Needle.RotateTo(direction, 50, Easing.SpringOut);
        });
    }
    private int GetDirection()
    {
        return weatherData.current.wind_deg;
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        if (aTimer == null)
            Start();
        else
            aTimer.Enabled = !aTimer.Enabled;
    }
}