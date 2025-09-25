using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = false;

        public MainWindow()
        {
            Resources.Add("BoolToLightConverter", new BoolToLightConverter());
            Resources.Add("BoolToBackgroundConverter", new BoolToBackgroundConverter());

            InitializeComponent();
            AddTestRooms();
        }

        private void AddTestRooms()
        {
            AddRoomControl(new Room { Name = "Гостиная", Temperature = 22.5, IsLightOn = true });
            AddRoomControl(new Room { Name = "Спальня", Temperature = 20.0, IsLightOn = false });
            AddRoomControl(new Room { Name = "Кухня", Temperature = 23.0, IsLightOn = true });
        }

        private async void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Введите название комнаты:", "Новая комната");
            if (inputDialog.ShowDialog() == true)
            {
                var room = new Room { Name = inputDialog.Answer, Temperature = 21.0, IsLightOn = false };
                await AddRoomControl(room);
                StatusText.Text = $"Добавлена комната: {room.Name}";
            }
        }

        private async Task AddRoomControl(Room room)
        {
            var roomControl = new RoomControl(room);
            roomControl.DeleteRequested += (control) => RoomsPanel.Children.Remove(control);
            roomControl.EditRequested += (r) =>
            {
                var dialog = new InputDialog("Введите новое название:", r.Name);
                if (dialog.ShowDialog() == true) r.Name = dialog.Answer;
            };

            RoomsPanel.Children.Add(roomControl);
            await roomControl.AnimateAppearAsync();
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            Background = _isDarkTheme ? Brushes.DimGray : Brushes.WhiteSmoke;
            ThemeToggleButton.Content = _isDarkTheme ? "☀️ Светлая тема" : "🌙 Темная тема";
        }
    }

    public class Room : INotifyPropertyChanged
    {
        private string _name = "Комната";
        private bool _isLightOn;
        private double _temperature = 21.0;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public bool IsLightOn
        {
            get => _isLightOn;
            set { _isLightOn = value; OnPropertyChanged(nameof(IsLightOn)); }
        }

        public double Temperature
        {
            get => _temperature;
            set { _temperature = value; OnPropertyChanged(nameof(Temperature)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RoomControl : Border
    {
        public Room CurrentRoom { get; set; }
        public event Action<RoomControl> DeleteRequested;
        public event Action<Room> EditRequested;

        public RoomControl(Room room)
        {
            CurrentRoom = room;
            InitializeComponent();
            DataContext = CurrentRoom;
        }

        private void InitializeComponent()
        {
            BorderBrush = Brushes.Gray;
            BorderThickness = new Thickness(1);
            CornerRadius = new CornerRadius(5);
            Margin = new Thickness(5);
            Width = 250;
            Height = 150;
            Background = new LinearGradientBrush(Colors.LightGray, Colors.White, 45);

            var grid = new Grid { Margin = new Thickness(5) };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var nameText = new TextBlock
            {
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Text = CurrentRoom.Name
            };
            nameText.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            Grid.SetRow(nameText, 0);
            grid.Children.Add(nameText);

            var tempText = new TextBlock();
            tempText.SetBinding(TextBlock.TextProperty,
                new Binding("Temperature") { StringFormat = "Температура: {0:F2}°C" });
            Grid.SetRow(tempText, 1);
            grid.Children.Add(tempText);

            var slider = new Slider { Minimum = 15, Maximum = 30, Margin = new Thickness(10, 10, 10, 10) };
            slider.SetBinding(Slider.ValueProperty, new Binding("Temperature"));
            Grid.SetRow(slider, 2);
            grid.Children.Add(slider);

            var lightText = new TextBlock();
            lightText.SetBinding(TextBlock.TextProperty,
                new Binding("IsLightOn") { Converter = new BoolToLightConverter() });
            Grid.SetRow(lightText, 3);
            grid.Children.Add(lightText);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var lightBtn = new Button { Content = "💡 Свет", Margin = new Thickness(2) };
            lightBtn.Click += (s, e) => CurrentRoom.IsLightOn = !CurrentRoom.IsLightOn;

            var editBtn = new Button { Content = "✏️ Изменить", Margin = new Thickness(2) };
            editBtn.Click += (s, e) => EditRequested?.Invoke(CurrentRoom);

            var deleteBtn = new Button { Content = "🗑️ Удалить", Margin = new Thickness(2) };
            deleteBtn.Click += async (s, e) =>
            {
                await AnimateDeleteAsync();
                DeleteRequested?.Invoke(this);
            };

            buttonPanel.Children.Add(lightBtn);
            buttonPanel.Children.Add(editBtn);
            buttonPanel.Children.Add(deleteBtn);
            Grid.SetRow(buttonPanel, 4);
            grid.Children.Add(buttonPanel);

            Child = grid;

            SetBinding(BackgroundProperty,
                new Binding("IsLightOn") { Converter = new BoolToBackgroundConverter() });
        }

        public async Task AnimateDeleteAsync()
        {
            for (double scale = 1.0; scale > 0.1; scale -= 0.1)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    RenderTransform = new ScaleTransform(scale, scale);
                    Opacity = scale;
                });
                await Task.Delay(50);
            }
        }

        public async Task AnimateAppearAsync()
        {
            Opacity = 0;
            for (double opacity = 0; opacity <= 1.0; opacity += 0.1)
            {
                await Dispatcher.InvokeAsync(() => Opacity = opacity);
                await Task.Delay(30);
            }
        }
    }

    public class BoolToLightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => (bool)value ? "Свет ВКЛ" : "Свет ВЫКЛ";

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ?
                new LinearGradientBrush(Colors.LightYellow, Colors.White, 45) :
                new LinearGradientBrush(Colors.LightGray, Colors.White, 45);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class InputDialog
    {
        public string Answer { get; set; }

        public InputDialog(string question, string defaultAnswer = "")
        {
            var dialog = new Window()
            {
                Title = "Ввод названия",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var textBox = new TextBox { Text = defaultAnswer, Margin = new Thickness(10) };
            var okButton = new Button { Content = "OK", Width = 80, Margin = new Thickness(5) };

            okButton.Click += (s, e) => { Answer = textBox.Text; dialog.DialogResult = true; };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = question, Margin = new Thickness(10) });
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(okButton);

            dialog.Content = stackPanel;
            dialog.ShowDialog();
        }

        public bool ShowDialog() => Answer != null;
    }
}
