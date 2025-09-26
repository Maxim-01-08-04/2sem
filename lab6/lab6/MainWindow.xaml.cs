using Microsoft.Win32;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace lab6
{
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
        }

        private void SelectDatabaseClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "SQLite Database (*.db;*.sqlite)|*.db;*.sqlite|All files (*.*)|*.*";
            dialog.Title = "Выберите файл базы данных";

            if (dialog.ShowDialog() == true)
            {
                dbManager.ConnectionString = $"Data Source={dialog.FileName}";
                DbPathText.Text = System.IO.Path.GetFileName(dialog.FileName);

                

                MessageBox.Show("База данных успешно подключена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshDataClick(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            if (dbManager.IsConnected())
            {
                ApartmentsGrid.ItemsSource = dbManager.GetApartments();
                RoomsGrid.ItemsSource = dbManager.GetRoomsWithInfo();
            }
        }


        private void DeleteApartmentClick(object sender, RoutedEventArgs e)
        {
            var apartment = (Apartment)((FrameworkElement)sender).DataContext;
            dbManager.DeleteApartment(apartment.Id);
            RefreshData();
        }

        private void DeleteRoomClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var room = button.DataContext as Room; 

            if (room != null)
            {
                dbManager.DeleteRoom(room.Id);
                RefreshData();
            }
        }

        private void SaveChangesClick(object sender, RoutedEventArgs e)
        {
            ApartmentsGrid.CommitEdit();
            RoomsGrid.CommitEdit();

            var apartments = ApartmentsGrid.ItemsSource.Cast<Apartment>().Where(a => a != null).ToList();
            var rooms = RoomsGrid.ItemsSource.Cast<Room>().Where(r => r != null).ToList();
            foreach (var apt in apartments)
            {
                if (string.IsNullOrEmpty(apt.ApartmentNumber))
                {
                    MessageBox.Show("Заполните номер квартиры");
                    return;
                }
            }
            if (dbManager.IsConnected())
            {
                dbManager.SaveApartments(ApartmentsGrid.ItemsSource.Cast<Apartment>().ToList());
                dbManager.SaveRooms(RoomsGrid.ItemsSource.Cast<Room>().ToList());
                RefreshData();
            }
        }
    }
}
