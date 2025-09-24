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

namespace _1_lab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentInput = ""; 
        private double firstNumber = 0;  
        private string operation = "";   
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            currentInput += button.Content.ToString();
            Display.Text = currentInput;
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (!string.IsNullOrEmpty(currentInput))
            {
                firstNumber = double.Parse(currentInput);
                operation = button.Content?.ToString() ?? string.Empty;
                currentInput = "";
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput) && !string.IsNullOrEmpty(operation))
            {
                double secondNumber = double.Parse(currentInput);
                double result = 0;

                try
                {
                    switch (operation)
                    {
                        case "+":
                            result = firstNumber + secondNumber;
                            break;
                        case "-":
                            result = firstNumber - secondNumber;
                            break;
                        case "*":
                            result = firstNumber * secondNumber;
                            break;
                        case "/":
                            if (secondNumber == 0)
                            {
                                throw new DivideByZeroException("Division by zero is not allowed.");
                            }
                            result = firstNumber / secondNumber;
                            break;
                    }

                    Display.Text = result.ToString();
                    currentInput = result.ToString();
                }
                catch (DivideByZeroException ex)
                {
                    Display.Text = "Error";
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            firstNumber = 0;
            operation = "";
            Display.Text = "0";
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                Display.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
            }
        }
    }
}