using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ActiviaAPP.Classes;
using ActiviaAPP.Popups;

namespace ActiviaAPP
{
    public partial class User : Page, INotifyPropertyChanged
    {
        // Attributter
        private string? _username;
        
        public string? username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? userPassword;
        public string? userFullName;
        public string? userMail;
        public int userPhone;

        public User()
        {
            InitializeComponent();
            DataContext = this; 
            
            //Binder aktivitetslisten "ActivityStore" med aktivitetslisten hos medlem
            UserListbox.ItemsSource = ActivityStore.activities;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Login());
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UserListbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenSelectedActivityDetails();
        }

        private void OpenActivity(object sender, RoutedEventArgs e)
        {
            OpenSelectedActivityDetails();
        }

        /// <summary>
        /// Metode der viser den valgte aktivitet og dens detaljer
        /// </summary>
        private void OpenSelectedActivityDetails()
        {
            var activity = UserListbox.SelectedItem as ActivityClass;

            //If-sætning hvis ingen aktivitiet er valgt
            if (activity == null)
            {
                //Popup vindue der fortæller at der skal vælges en aktivitet
                MessageBox.Show("Vælg en aktivitet først");
                return;
            }

            //Den valgte aktivitets detaljer vises
            var userId = string.IsNullOrWhiteSpace(username) ? userFullName : username;
            var dlg = new ActivityDetails(activity, userId ?? string.Empty);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Metode der håndterer tilmeldingsprocessen for et medlem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignUpActivity(object sender, RoutedEventArgs e) 
        {
            var activity = UserListbox.SelectedItem as ActivityClass;

           //If-sætning i tilfælde af at ingen aktivitet er valgt
            if (activity == null)
            {
                //Popup vindue som fortæller at medlem skal vælge en aktivitet
                MessageBox.Show("Vælg en aktivitet");
                return;
            }

            var userID = string.IsNullOrWhiteSpace(username) ? userFullName : username;
            
            //If-sætning i tilfælde af at medlem intet brugernavn "userID" har
            if (string.IsNullOrWhiteSpace(userID))
            {
                MessageBox.Show("Angiv dit brugernavn"); 
                return;
            }

            var success = activity.Register(userID);

            //If-sætning når medlem er succesfuldt tilmeldt en aktivitet
            if (success)
            {
                //Popup vindue der fortæller medlem at de er tilmeldt aktiviteten
                MessageBox.Show($"Du er tilmeldt: {activity.ActivityTitle}");
            }
            else
            {
               //If-sætning i tilfælde af, at medlem allerede er tilmeldt aktiviteten
                if (activity.RegisteredUsers.Contains(userID))
                    MessageBox.Show("Du er allerede tilmeldt denne aktivitet");

                //Else if-sætning i tilfælde af max antal deltagere
                else if (activity.MaxParticipants > 0 && activity.CurrentParticipantCount >= activity.MaxParticipants)
                    MessageBox.Show("Aktiviten er fuld");

                //Else-sætning der melder fejl og at medlem ikke kunne tilmelde sig aktiviteten
                else
                    MessageBox.Show("Beklager, du kunne ikke tilmeldes til aktiviteten"); 
            }

        }
    }
}
