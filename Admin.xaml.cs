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
    public partial class Admin : Page
    {
        // Attributter
        public string adminName;
        public string adminPassword;
        public string adminCompany;

        public Admin()
        {
            InitializeComponent();
        }

        private void activityList(object sender, SelectionChangedEventArgs e)
        {
            // Her skal aktiviteter vises
        }

        private void addActivity(object sender, RoutedEventArgs e)
        {
            var createActivity = new ActiviaAPP.Popups.CreateActivity();
            createActivity.ShowDialog();
        }

        private void removeActivity(object sender, RoutedEventArgs e)
        {
      
        }

        private void adminSettings(object sender, RoutedEventArgs e)
        {
  
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }

    }
}
