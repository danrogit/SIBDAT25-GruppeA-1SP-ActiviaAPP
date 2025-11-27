using ActiviaAPP.Classes;
using ActiviaAPP.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window
    {
        /*ActivityClass activity = new ActivityClass();
        public ObservableCollection<ActivityClass> Activities { get; set; } = new ObservableCollection<ActivityClass>();
        
        public Admin? adminPage;*/

        public MainWindow()
        {
            InitializeComponent();

          
            // Laver startsiden om til login
            MainFrame.Navigate(new Login());
           

            /* List<Activity> activities = new List<Activity>();
            activityList.ItemsSource = Activities; */
        }
      
    }
}
