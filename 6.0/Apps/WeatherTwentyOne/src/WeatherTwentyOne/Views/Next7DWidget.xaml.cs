using WeatherTwentyOne.Models;
using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Views;

public partial class Next7DWidget
{
    public OpenWeatherMapModel weatherData;
    public Next7DWidget(OpenWeatherMapModel weatherData)
    {
        InitializeComponent();

        BindingContext = weatherData;
    }
}
