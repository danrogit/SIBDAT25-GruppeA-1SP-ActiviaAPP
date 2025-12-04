using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ActiviaAPP.Popups
{
    public partial class CreateActivity : Window
    {
        // Public felter som bruges af kaldende kode til at hente de indtastede værdier
        public string ActivityTitle = string.Empty;
        public string ActivityType = string.Empty;
        public DateTime ActivityDate;
        public string ActivityDescription = string.Empty;
        public int MaxParticipants;
        public string CoverImagePath = string.Empty;

        public CreateActivity()
        {
            InitializeComponent();
        }

        // Håndter opdatering af slider-værdi: vis den aktuelle værdi i ParticipantValue TextBlock
        private void participantValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ParticipantValue != null)
                ParticipantValue.Text = ((int)ParticipantSlider.Value).ToString();
        }

        // Åbn filvælger for at vælge et cover-billede og vis preview i PreviewImage
        private void imgClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Filtyper (*.jpg; *.png)|*.jpg;*.png" };
            if (dialog.ShowDialog() != true) return;

            CoverImagePath = dialog.FileName;

            // Hvis PreviewImage kontrol ikke findes, spring preview-logikken over
            if (PreviewImage == null) return;

            try
            {
                // Nulstil preview før indlæsning
                PreviewImage.Source = null;
                PreviewImage.Visibility = Visibility.Collapsed;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                // OnLoad så filen kan være lukket efter indlæsning
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(CoverImagePath, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze(); // Gør billedet trådsikkert

                // Tildel billedet og gør preview synligt
                PreviewImage.Source = bmp;
                PreviewImage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Hvis noget går galt ved indlæsning af preview, skjul preview og vis fejl
                PreviewImage.Source = null;
                PreviewImage.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Kunne ikke indlæse preview-billede: {ex.Message}", "Fejl", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Håndter klik på "Opret aktivitet" - valider input, sæt properties og luk dialogen med DialogResult = true
        private void createClick(object sender, RoutedEventArgs e)
        {
            // Simpel validering: alle felter skal udfyldes
            if (string.IsNullOrEmpty(TitleBox.Text) ||
                TypeBox.SelectedItem == null ||
                DateBox.SelectedDate == null ||
                string.IsNullOrEmpty(DescriptionBox.Text))
            {
                MessageBox.Show("Alle felter skal udfyldes");
                return;
            }

            // Overfør værdier fra UI til public felter
            ActivityTitle = TitleBox.Text;
            ActivityType = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            ActivityDate = DateBox.SelectedDate ?? DateTime.MinValue;
            ActivityDescription = DescriptionBox.Text;
            MaxParticipants = (int)ParticipantSlider.Value;

            // Hvis intet billede er valgt, sæt en informativ tekst (kan ændres til tom sti hvis ønsket)
            if (string.IsNullOrEmpty(CoverImagePath))
                CoverImagePath = "Intet billede er valgt";

            // Bekræft oprettelse for brugeren og lukk dialogen
            MessageBox.Show("Aktivitet blev oprettet");
            this.DialogResult = true;
            this.Close();
        }

        // Annuller oprettelse: luk dialogen med DialogResult = false
        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Tom event handler for TitleBox.TextChanged (kan bruges til live-validering)
        private void titleChange(object sender, TextChangedEventArgs e) { }
    }
}
