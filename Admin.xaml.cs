using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Admin : Page
    {
        // Attributter
        public string adminName;
        public string adminPassword;
        public string adminCompany;
      

        private readonly ObservableCollection<ActivityClass> activities = new ObservableCollection<ActivityClass>();

        public Admin()
        {
            InitializeComponent();

            //Binder ActivityStore til listen i Admin
            ActivityListBox.ItemsSource = ActivityStore.activities;
        }
        internal static ObservableCollection<ActivityClass> Userlists()
        {
            return ActivityStore.activities;
        }
            

        private void activityList(object sender, SelectionChangedEventArgs e)
        {
            // Her skal aktiviteter vises
        }

       
        private void addActivity(object sender, RoutedEventArgs e)
        {
            var createActivity = new ActiviaAPP.Popups.CreateActivity();
            var result = createActivity.ShowDialog();

            if (result == true)
            {
                var activity = new ActivityClass
                {
                    ActivityTitle = createActivity.ActivityTitle,
                    ActivityType = createActivity.ActivityType,
                    Date = createActivity.ActivityDate,
                    Description = createActivity.ActivityDescription,
                    MaxParticipants = createActivity.MaxParticipants,
                    CoverImagePath = createActivity.CoverImagePath
                };
                // Tilføj den nye aktivitet til den delte liste
               ActivityStore.activities.Add(activity);
            }                                     
            
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

