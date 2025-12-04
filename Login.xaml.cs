using ActiviaAPP.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        public Login()
        {
            InitializeComponent();
        }

        private void logIn(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            // Valider at brugernavn og kode er udfyldt
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Indtast brugernavn og adgangskode");
                return;
            }

            // Demo admin-login (hardcoded)
            if (user == "admin" && pass == "1")
            {
                // Gå videre til admin-siden
                NavigationService?.Navigate(new Admin());
                return;
            }

            // Forsøg at finde en oprettet bruger i UserStore
            var foundUser = UserStore.FindUser(user, pass);
            if (foundUser != null)
            {
                // Opret User-side og sæt brugerdata før navigation
                var userPage = new User
                {
                    username = foundUser.Username,
                    userFullName = foundUser.FullName,
                    userMail = foundUser.Email,
                    userPhone = int.TryParse(foundUser.Phone, out int phone) ? phone : 0
                };
                NavigationService?.Navigate(userPage);
                return;
            }

            // Demo almindelig user-login (hardcoded)
            else if (user == "user" && pass == "1")
            {
                // Gå videre til user-siden
                NavigationService?.Navigate(new User());
                return;
            }
            else
            {
                // Forkert credentials
                MessageBox.Show("Forkert brugernavn eller adgangskode");
            }
        }

        // Håndter "Upload CSV"-knap: åbner filvælger og kalder UserStore.LoadFromCsv
        private void UploadCsv(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV filer (*.csv)|*.csv|Alle filer (*.*)|*.*",
                Title = "Vælg en CSV fil med brugere"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Indlæs brugere fra den valgte fil
                    UserStore.LoadFromCsv(openFileDialog.FileName);
                    MessageBox.Show($"Brugere er blevet indlæst fra filen!\n\nAntal brugere: {UserStore.RegisteredUsers.Count}", 
                                    "Success", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Vis fejl ved indlæsning
                    MessageBox.Show($"Kunne ikke indlæse CSV filen:\n{ex.Message}", 
                                    "Fejl", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                }
            }
        }

        // Naviger til SignUp-siden hvis brugeren vil oprette en ny bruger
        private void GoToSignUp(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SignUp());
        }
    }
}
