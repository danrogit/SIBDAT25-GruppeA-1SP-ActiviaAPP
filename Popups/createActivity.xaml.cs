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
using System.Windows.Shapes;

namespace ActiviaAPP.Popups
{
    public partial class CreateActivity : Window   // FIX: uppercase C
    {
        public CreateActivity()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Today; // dato felt som er en variabel
        }

        private void Titel_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActivityClass myActivity = new ActivityClass();
            myActivity.activityTitle = Titel.Text;
        }

        private void Type_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActivityClass myActivity = new ActivityClass();
            myActivity.activityType = Type.Text;
        }
        private void almTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActivityClass myActivity = new ActivityClass();
            myActivity.activityType = almTextBox.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dato = DatePicker.SelectedDate ?? DateTime.Today;
        }
    }
}
