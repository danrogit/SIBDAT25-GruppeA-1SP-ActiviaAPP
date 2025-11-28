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
using ActiviaAPP.Classes;

namespace ActiviaAPP
{
    public partial class User : Page
    {
        // Attributter
        public string username;
        public string userPassword;
        public string userFullName;
        public string userMail;
        public int userPhone;

        public User()
        {
            InitializeComponent();
            UserListbox.ItemsSource = ActivityStore.activities;
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            // Gå til startsiden
            NavigationService?.Navigate(new Login());
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }
    }
}
