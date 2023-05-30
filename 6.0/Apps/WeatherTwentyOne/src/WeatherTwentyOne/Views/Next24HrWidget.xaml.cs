using WeatherTwentyOne.Models;
using WeatherTwentyOne.ViewModels;

namespace WeatherTwentyOne.Views;

public partial class Next24HrWidget
{
    public OpenWeatherMapModel weatherData;

    public Next24HrWidget(OpenWeatherMapModel weatherData)
    {
        InitializeComponent();

        BindingContext = weatherData;
    }
}
