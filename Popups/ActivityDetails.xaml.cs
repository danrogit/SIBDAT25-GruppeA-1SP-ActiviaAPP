using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ActiviaAPP.Classes;
using System.IO;

namespace ActiviaAPP.Popups
{
    public partial class ActivityDetails : Window
    {
        private readonly ActivityClass activity;
        private readonly string userId;

        public ActivityDetails(ActivityClass activity, string currentUserId)
        {
            InitializeComponent();

            // Gem de indkomne parametre og sørg for at activity ikke er null
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
            this.userId = currentUserId ?? string.Empty;

            // Initial population af UI med data fra activity
            Populate();

            // Abonner på ændringer i RegisteredUsers så UI opdateres når tilmeldinger ændres
            activity.RegisteredUsers.CollectionChanged += (s, e) => Dispatcher.Invoke(Populate);
        }

        // Opdater UI-elementer med information fra activity
        private void Populate()
        {
            TitleText.Text = activity.ActivityTitle;
            DescriptionText.Text = activity.Description;
            DateText.Text = activity.Date.ToString("dd-MM-yyyy");
            ParticipantsText.Text = $"{activity.CurrentParticipantCount}/{(activity.MaxParticipants > 0 ? activity.MaxParticipants.ToString() : "∞")}";
            RegisteredList.Text = "Tilmeldte: " + (activity.RegisteredUsers.Count == 0 ? "Ingen" : string.Join(", ", activity.RegisteredUsers));

            // Billedhåndtering: understøtter absolutte filstier, app-relativ sti og pack:// URIs
            var path = activity.CoverImagePath ?? string.Empty;
            BitmapImage? img = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    // Hvis det er en absolut filsti på disk
                    if (Path.IsPathRooted(path) && File.Exists(path))
                    {
                        img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri(path, UriKind.Absolute);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                    }
                    // Hvis det er en pack URI (resource embedded i applikationen)
                    else if (path.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
                    {
                        img = new BitmapImage(new Uri(path, UriKind.Absolute));
                    }
                    // Ellers forsøg at fortolke stien som relativ til app-basen
                    else
                    {
                        var appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart('\\', '/').Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(appPath))
                        {
                            img = new BitmapImage();
                            img.BeginInit();
                            img.UriSource = new Uri(appPath, UriKind.Absolute);
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.EndInit();
                        }
                    }
                }
            }
            catch
            {
                // Hvis indlæsning mislykkes, lad img være null (ingen preview)
                img = null;
            }

            // Sæt cover-billedet (kan være null)
            CoverImage.Source = img;

            // Opdater tilmeld-knap afhængig af om brugeren allerede er tilmeldt og om aktiviteten er fuld
            if (activity.RegisteredUsers.Contains(userId))
            {
                RegisterBtn.Content = "Tilmeldt";
                RegisterBtn.IsEnabled = false;
            }
            else
            {
                RegisterBtn.Content = "Tilmeld";
                RegisterBtn.IsEnabled = activity.MaxParticipants == 0 || activity.CurrentParticipantCount < activity.MaxParticipants;
            }
        }

        // Håndter klik på Tilmeld-knap: til- eller afmeld bruger
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (activity.RegisteredUsers.Contains(userId))
            {
                // Hvis allerede tilmeldt -> forsøg at afmelde
                var removed = activity.Unregister(userId);
                if (removed) MessageBox.Show("Du er afmeldt aktiviteten");
            }
            else
            {
                // Forsøg at tilmelde bruger
                var ok = activity.Register(userId);
                if (ok) MessageBox.Show("Du er tilmeldt aktiviteten");
                else MessageBox.Show("Kunne ikke tilmelde - aktiviteten kan være fuld eller du er allerede tilmeldt");
            }

            // Opdater UI efter handling
            Populate();
        }

        // Luk vinduet
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}
