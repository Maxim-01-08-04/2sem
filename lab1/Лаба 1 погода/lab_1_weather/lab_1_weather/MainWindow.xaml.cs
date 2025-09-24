using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace lab_1_weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            StartTimer(); 
            InitializeData(); 
        }

        private void StartTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += Timer_Tick; 
            timer.Start(); 
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeText.Text = $"Сейчас {DateTime.Now.ToString("HH:mm")}. Вчера в это время 8";
        }

        private void InitializeData()
        {
            CityName.Text = "Район, Новосибирск";
            TemperatureText.Text = "15";
            WindText.Text = "1,0 м/с, Ю";
            HumidityText.Text = "95%";
            PressureText.Text = "766 мм рт. ст.";
            WeatherIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/sun.png", UriKind.Relative));
        }

    }
}