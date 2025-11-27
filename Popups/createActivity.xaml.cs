using Microsoft.Win32;
﻿using ActiviaAPP.Classes;
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
    public partial class CreateActivity : Window
    {
        // Simple variabler der gemmer inputtet
        public string ActivityTitle;
        public string ActivityType;
        public DateTime ActivityDate;
        public string ActivityDescription;
        public int MaxParticipants;
        public string CoverImagePath;

        public CreateActivity()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Today; // dato felt som er en variabel
        }

        private void participantValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Opdater tal til højre for slider (deltagerbegrænsning)
            if (ParticipantValue != null)
            {
                ParticipantValue.Text = ((int)ParticipantSlider.Value).ToString();
            }
        }

        private void imgClick(object sender, RoutedEventArgs e)
        {
            // 
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Filtyper (*.jpg; *.png)|*.jpg;*.png";  // Brugeren ser filtype, resten bestemmer hvilke typer der må bruges

            if (dialog.ShowDialog() == true)
            {
                CoverImagePath = dialog.FileName;

                // Vis det valgte billede som et preview
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(CoverImagePath);
                img.EndInit();

                PreviewImage.Source = img;
            }
        }

        private void createClick(object sender, RoutedEventArgs e)
        {
            // validering af at alt er valgt
            if (TitleBox.Text == "" ||
                TypeBox.SelectedItem == null ||
                DateBox.SelectedDate == null ||
                DescriptionBox.Text == "")
            {
                MessageBox.Show("Alle felter skal udfyldes");
                return;
            }

            // Gem værdier i variabler
            ActivityTitle = TitleBox.Text;
            ActivityType = (TypeBox.SelectedItem as ComboBoxItem).Content.ToString();
            ActivityDate = DateBox.SelectedDate ?? DateTime.MinValue;
            ActivityDescription = DescriptionBox.Text;
            MaxParticipants = (int)ParticipantSlider.Value;

            // Selv hvis billede ikke vælges
            if (CoverImagePath == null)
            {
                CoverImagePath = "Intet billede er valgt";
            }

            MessageBox.Show("Aktivitet blev oprettet");

            this.DialogResult = true;   // Bruges i Admin til at se om popup blev gemt
            this.Close();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void titleChange(object sender, TextChangedEventArgs e)
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
