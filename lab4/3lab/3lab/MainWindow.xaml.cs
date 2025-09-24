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

namespace _3lab
{
    
    public partial class MainWindow : Window
    {
        private List<Room> _rooms = new List<Room>();

        public MainWindow()
        {
            InitializeComponent();

            _rooms.Add(new Room { Number = 101, Name = "Гостиная", Area = 25.5 });
            _rooms.Add(new Room { Number = 102, Name = "Спальня", Area = 18.0 });
            _rooms.Add(new Room { Number = 103, Name = "Кухня", Area = 12.3 });

            RefreshRoomsList();
            AddLog("Приложение запущено");
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            var room = new Room();
            room.RoomPropertyChanged += OnRoomPropertyChanged;

            if (ShowRoomEditor(room))
            {
                _rooms.Add(room);
                RefreshRoomsList();
                AddLog($"Добавлена комната: {room}");
            }
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsListBox.SelectedItem is Room selectedRoom)
            {
                if (ShowRoomEditor(selectedRoom))
                {
                    RefreshRoomsList();
                    AddLog($"Изменена комната: {selectedRoom}");
                }
            }
            else
            {
                MessageBox.Show("Выберите комнату для редактирования");
            }
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsListBox.SelectedItem is Room selectedRoom)
            {
                if (MessageBox.Show($"Удалить комнату {selectedRoom}?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _rooms.Remove(selectedRoom);
                    RefreshRoomsList();
                    AddLog($"Удалена комната: {selectedRoom}");
                }
            }
            else
            {
                MessageBox.Show("Выберите комнату для удаления");
            }
        }

        private bool ShowRoomEditor(Room room)
        {
            var editWindow = new Window()
            {
                Title = "Редактирование комнаты",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var grid = new Grid { Margin = new Thickness(10) };

            for (int i = 0; i < 6; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = i == 4 ? new GridLength(1, GridUnitType.Star) : GridLength.Auto
                });
            }
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            var numberLabel = new TextBlock { Text = "Номер:", Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            var numberBox = new TextBox { Margin = new Thickness(5), Text = room.Number.ToString() };

            var nameLabel = new TextBlock { Text = "Название:", Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            var nameBox = new TextBox { Margin = new Thickness(5), Text = room.Name };

            var areaLabel = new TextBlock { Text = "Площадь (м²):", Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
            var areaBox = new TextBox { Margin = new Thickness(5), Text = room.Area.ToString() };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var saveButton = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
            var cancelButton = new Button { Content = "Отмена", Width = 80, Margin = new Thickness(5) };

            Grid.SetRow(numberLabel, 0); Grid.SetColumn(numberLabel, 0);
            Grid.SetRow(numberBox, 0); Grid.SetColumn(numberBox, 1);
            Grid.SetRow(nameLabel, 1); Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameBox, 1); Grid.SetColumn(nameBox, 1);
            Grid.SetRow(areaLabel, 2); Grid.SetColumn(areaLabel, 0);
            Grid.SetRow(areaBox, 2); Grid.SetColumn(areaBox, 1);
            Grid.SetRow(buttonPanel, 5); Grid.SetColumnSpan(buttonPanel, 2);

            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(cancelButton);

            grid.Children.Add(numberLabel);
            grid.Children.Add(numberBox);
            grid.Children.Add(nameLabel);
            grid.Children.Add(nameBox);
            grid.Children.Add(areaLabel);
            grid.Children.Add(areaBox);
            grid.Children.Add(buttonPanel);

            editWindow.Content = grid;

            bool isSaved = false;

            saveButton.Click += (s, e) =>
            {
                if (int.TryParse(numberBox.Text, out int number) && double.TryParse(areaBox.Text, out double area))
                {
                    room.Number = number;
                    room.Name = nameBox.Text;
                    room.Area = area;
                    isSaved = true;
                    editWindow.Close();
                }
                else
                {
                    MessageBox.Show("Проверьте правильность ввода данных");
                }
            };

            cancelButton.Click += (s, e) => editWindow.Close();

            editWindow.ShowDialog();
            return isSaved;
        }

        private void RefreshRoomsList()
        {
            RoomsListBox.ItemsSource = null;
            RoomsListBox.ItemsSource = _rooms;
        }

        private void AddLog(string message)
        {
            LogListBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            if (LogListBox.Items.Count > 0)
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
        }

        private void OnRoomPropertyChanged(Room room, string propertyName, string newValue, DateTime eventTime)
        {
            Dispatcher.Invoke(() =>
            {
                AddLog($"[{eventTime:HH:mm:ss}] {room.Name}: свойство {propertyName} = {newValue}");
            });
        }

        private void RoomsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusText.Text = RoomsListBox.SelectedItem != null
                ? $"Выбрана: {RoomsListBox.SelectedItem}"
                : "Готово";
        }
    }

    public class Room : INotifyPropertyChanged
    {
        private string _name = "Новая комната";
        private double _area = 10.0;
        private int _number = 1;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnRoomPropertyChanged(nameof(Name), value);
                }
            }
        }

        public double Area
        {
            get => _area;
            set
            {
                if (_area != value)
                {
                    _area = value;
                    OnPropertyChanged(nameof(Area));
                    OnRoomPropertyChanged(nameof(Area), value.ToString("F1"));
                }
            }
        }

        public int Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                    OnRoomPropertyChanged(nameof(Number), value.ToString());
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void RoomPropertyChangedDelegate(Room room, string propertyName, string newValue, DateTime eventTime);
        public event RoomPropertyChangedDelegate RoomPropertyChanged;

        protected virtual void OnRoomPropertyChanged(string propertyName, string newValue)
        {
            RoomPropertyChanged?.Invoke(this, propertyName, newValue, DateTime.Now);
        }

        public override string ToString()
        {
            return $"{Number} - {Name} ({Area} м²)";
        }
    }
}
