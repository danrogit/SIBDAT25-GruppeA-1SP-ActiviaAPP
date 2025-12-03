using ActiviaAPP.Classes;
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
    public partial class SignUp : Page
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void CreateUser(object sender, RoutedEventArgs e)
        {
            // Tjekker om alle felter er udfyldt før man kan gå videre
            if (string.IsNullOrWhiteSpace(FullNameBox.Text) ||
                string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("Udfyld alle felter");
                return;
            }

            // Tjek om adgangskoderne matcher i oprettelsen
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Adgangskoden matcher ikke");
                return;
            }


            // Tjek om brugernavnet allerede findes
            if (UserStore.UsernameExists(UsernameBox.Text))
            {
                MessageBox.Show("Brugernavnet er allerede taget");
                return;
            }


            // Opretter en ny bruger
            var newUser = new ActiviaAPP.Classes.User
            {
                FullName = FullNameBox.Text,
                Username = UsernameBox.Text,
                Email = EmailBox.Text,
                Phone = PhoneBox.Text,
                Password = PasswordBox.Password
            };


            // Gemmer brugeren i UserStore.cs filen
            var success = UserStore.RegisterUser(newUser); 
            if (success)
            {
                MessageBox.Show($"{newUser.Username} blev oprettet. Du kan nu logge ind");

                NavigationService?.Navigate(new Login());
            }
            else
            {
                MessageBox.Show("Brugeren blev ikke oprettet, prøv igen");
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            // Gå tilbage til login siden uden at gemme
            NavigationService?.Navigate(new Login());
        }
    }
}
