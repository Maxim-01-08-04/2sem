using System.Globalization;
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

namespace lab_1_cakendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime currentDisplayDate;
        public MainWindow()
        {
            InitializeComponent();
            currentDisplayDate = DateTime.Now; 
            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            int selectedYear = currentDisplayDate.Year;
            int selectedMonth = currentDisplayDate.Month;

            MonthYearLabel.Text = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth)} {selectedYear}";

            DateTime firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek == 0 ? 7 : (int)firstDayOfMonth.DayOfWeek;

            DaysGrid.Children.Clear();

            for (int i = 1; i < firstDayOfWeek; i++)
            {
                TextBlock emptyBlock = new TextBlock { Text = "", FontSize = 14, Foreground = Brushes.Gray };
                DaysGrid.Children.Add(emptyBlock);
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                TextBlock dayBlock = new TextBlock
                {
                    Text = day.ToString(),
                    FontSize = 14,
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5)
                };

                if ((firstDayOfWeek + day - 1) % 7 == 6 || (firstDayOfWeek + day - 1) % 7 == 0)
                {
                    dayBlock.Foreground = Brushes.Red;
                }

                DaysGrid.Children.Add(dayBlock);
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDisplayDate = currentDisplayDate.AddMonths(-1); 
            UpdateCalendar();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDisplayDate = currentDisplayDate.AddMonths(1); 
            UpdateCalendar();
        
    }
    }
}