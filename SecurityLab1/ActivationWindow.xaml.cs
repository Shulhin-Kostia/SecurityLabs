using System.Windows;
using System.Text.Json;
using System.IO;

namespace SecurityLab1
{
    public partial class ActivationWindow : Window
    {
        private Activation _activation;

        public ActivationWindow()
        {
            InitializeComponent();
            _activation = JsonSerializer.Deserialize<Activation>(File.ReadAllText("Activation.json"));
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_activation.CheckPassword(passwordField.Password))
            {
                MessageBoxResult tryAgain =
                      MessageBox.Show(
                      "Password is incorrect!\nDo you want to try again?",
                      "Activation",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Error);

                if (tryAgain == MessageBoxResult.Yes)
                    passwordField.Clear();
                else
                {
                    ((MainWindow)Application.Current.MainWindow).Activate();
                    Close();
                }
                    
            }
            else
            {
                MessageBox.Show(
                  "Program activated successfully!",
                  "Activation",
                  MessageBoxButton.OK,
                  MessageBoxImage.Information);
                Close();
            }
        }
    }
}
