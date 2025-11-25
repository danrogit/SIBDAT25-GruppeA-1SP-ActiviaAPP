using System;
using System.Collections.Generic;
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

namespace ActiviaAPP
{
    public partial class Login : Page
    {
        // Attributter

        public Login()
        {
            InitializeComponent();
        }

        private void logIn(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            // Demo admin login
            if (user == "admin" && pass == "1")
            {
                // Går videre til admin WPF siden
                NavigationService?.Navigate(new Admin());
            }
            // Demo user login
            else if (user == "user" && pass == "1")
            {
                // Går videre til user WPF siden
                NavigationService?.Navigate(new User());
            }
            else
            {
                MessageBox.Show("Forkert brugernavn eller adgangskode");
            }
        }
    }
}
