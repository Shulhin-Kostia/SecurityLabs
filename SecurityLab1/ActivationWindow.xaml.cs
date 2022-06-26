using System.Windows;
using System.Text.Json;
using System.IO;
using System.Windows.Input;

namespace SecurityLab1
{
    public partial class ActivationWindow : Window
    {
        private Activation _activation;
        private Captcha _captcha;

        public ActivationWindow()
        {
            InitializeComponent();
            _activation = JsonSerializer.Deserialize<Activation>(File.ReadAllText("Activation.json"));
            _captcha = new Captcha();
            captchaImage.Source = _captcha.GenerateCaptcha();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(captchaField.Text))
            {
                MessageBox.Show(
                      "CAPTCHA is empty!\nPlease enter something.",
                      "Activation",
                      MessageBoxButton.OK,
                      MessageBoxImage.Information);
            }
            else
            {
                if (!captchaField.Text.ToLower().Equals(_captcha.Text))
                {
                    MessageBox.Show(
                          "CAPTCHA is incorrect!\nPlease try again.",
                          "Activation",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);

                    captchaField.Clear();
                    captchaImage.Source = _captcha.GenerateCaptcha();
                }
                else
                {
                    if(string.IsNullOrEmpty(passwordField.Password))
                    {
                        MessageBox.Show(
                              "Password is empty!\nPlease enter something.",
                              "Activation",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                    }
                    else
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
                            {
                                passwordField.Clear();
                                captchaField.Clear();
                                captchaImage.Source = _captcha.GenerateCaptcha();
                            }
                            else
                            {
                                if (((MainWindow)Application.Current.MainWindow) != null)
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
        }
        private void changeImage_Click(object sender, MouseEventArgs e)
        {
            captchaImage.Source = _captcha.GenerateCaptcha();
        }

        
    }
}
