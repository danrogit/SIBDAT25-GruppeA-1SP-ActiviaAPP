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
using ActiviaAPP.Popups;
using Microsoft.Win32; // til OpenFileDialog
using System.IO;
using System.Globalization;
                                                    
namespace ActiviaAPP
{
    public partial class Admin : Page
    {
        // Attributter til administratordata (kun eksempler)
        public string adminName = string.Empty;
        public string adminPassword = string.Empty;
        public string adminCompany = string.Empty;
      
        // Lokalt ObservableCollection (ikke brugt aktivt - vi binder til ActivityStore.activities direkte)
        private readonly ObservableCollection<ActivityClass> activities = new ObservableCollection<ActivityClass>();

        public Admin()
        {
            InitializeComponent();

            // Binder ActivityStore.activities til ListBox'en i Admin-siden, så UI viser de aktuelle aktiviteter.
            ActivityListBox.ItemsSource = ActivityStore.activities;

            // Binder UserStore.RegisteredUsers (medlemsliste) til UI-listen, så medlemmer vises og opdateres.
            userList.ItemsSource = UserStore.RegisteredUsers;
        }

        internal static ObservableCollection<ActivityClass> Userlists()
        {
            // Returnerer referencen til den delte aktivitetsliste fra ActivityStore.
            return ActivityStore.activities;
        }
            
        private void activityList(object sender, SelectionChangedEventArgs e)
        {   
            // Valgfrit: opdater UI ved selection-change
        }

        private void addActivity(object sender, RoutedEventArgs e)
        {
            // Åbn popup-vinduet for oprettelse af aktivitet
            var createActivity = new ActiviaAPP.Popups.CreateActivity();
            var result = createActivity.ShowDialog();

            // Hvis brugeren bekræfter oprettelse (DialogResult == true), bygg ActivityClass og tilføj til ActivityStore
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
                // Tilføj den nye aktivitet til den delte liste, som UI er bundet til.
               ActivityStore.activities.Add(activity);
            }                                     
        }

        private void removeActivity(object sender, RoutedEventArgs e)
        {
            // Fjern den valgte aktivitet fra listen efter bekræftelse
            var selected = ActivityListBox.SelectedItem as ActivityClass;
            if (selected == null)
            {
                MessageBox.Show("Vælg en aktivitet først");
                return;
            }

            var confirm = MessageBox.Show($"Er du sikker på du vil slette '{selected.ActivityTitle}'?", "Bekræft", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                ActivityStore.activities.Remove(selected);
                MessageBox.Show($"Aktiviteten '{selected.ActivityTitle}' er slettet");
            }
        }

        // Åbn en filvælger og indlæs brugere via UserStore.LoadFromCsv
        private void adminSettings(object sender, RoutedEventArgs e)
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
                    // Brug den centraliserede LoadFromCsv metode i UserStore
                    UserStore.LoadFromCsv(openFileDialog.FileName);
                    MessageBox.Show($"Brugere er blevet indlæst fra filen!\n\nAntal brugere: {UserStore.RegisteredUsers.Count}",
                                    "Success",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kunne ikke indlæse CSV filen:\n{ex.Message}",
                                    "Fejl",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        // Upload af aktiviteter fra CSV (format: Title;Description;Date;MaxParticipants;ImagePath)
        private void UploadActivities(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "CSV filer (*.csv)|*.csv|Alle filer (*.*)|*.*",
                Title = "Vælg en CSV fil med aktiviteter"
            };

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                // Læs fil som UTF-8
                var lines = File.ReadAllLines(dlg.FileName, Encoding.UTF8);
                int added = 0;

                foreach (var raw in lines)
                {
                    if (string.IsNullOrWhiteSpace(raw))
                        continue;

                    var line = raw.Trim();

                    // Hvis linjen er omgivet af parenteser, fjern dem: "(...)" -> "..."
                    if (line.StartsWith("(") && line.EndsWith(")"))
                        line = line.Substring(1, line.Length - 2).Trim();

                    // Vælg delimiter: semikolon foretrækkes, ellers komma
                    char delimiter = line.Contains(';') ? ';' : ',';

                    // Split linjen og af-quote felterne
                    var fields = SplitLine(line, delimiter).Select(f => Unquote(f.Trim())).ToArray();

                    // Spring header-række over hvis første felt ligner "title"
                    var first = fields.Length > 0 ? fields[0].ToLowerInvariant() : string.Empty;
                    if (first == "title" || first == "titel")
                        continue;

                    if (fields.Length < 1)
                        continue;

                    // Map felter: Title;Description;Date;MaxParticipants;ImagePath
                    string title = fields.Length > 0 ? fields[0] : string.Empty;
                    string description = fields.Length > 1 ? fields[1] : string.Empty;
                    string dateStr = fields.Length > 2 ? fields[2] : string.Empty;
                    string maxStr = fields.Length > 3 ? fields[3] : string.Empty;
                    string imgPath = fields.Length > 4 ? fields[4] : string.Empty;

                    if (string.IsNullOrWhiteSpace(title))
                        continue;

                    // Parse dato: accepter yyyy-MM-dd først, ellers brug invariant parsing som fallback
                    if (!DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)
                        && !DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        // Hvis dato er ugyldig, sæt til i dag
                        date = DateTime.Today;
                    }

                    int maxParticipants = 0;
                    if (!int.TryParse(maxStr, out maxParticipants))
                        maxParticipants = 0;

                    // Undgå dubletter baseret på titel + dato
                    bool exists = ActivityStore.activities.Any(a =>
                        string.Equals(a.ActivityTitle, title, StringComparison.OrdinalIgnoreCase) && a.Date.Date == date.Date);

                    if (exists)
                        continue;

                    // Forsøg at løse billedstien (absolut, app-relativ eller pack URI)
                    var resolvedImage = ResolveImagePath(imgPath);

                    var activity = new ActivityClass
                    {
                        ActivityTitle = title,
                        Description = description,
                        Date = date,
                        MaxParticipants = maxParticipants,
                        CoverImagePath = resolvedImage
                    };

                    ActivityStore.activities.Add(activity);
                    added++;
                }

                MessageBox.Show($"Aktiviteter importeret: {added}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kunne ikke indlæse aktivitets-CSV:\n{ex.Message}", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Forsøg at producere en brugbar billedsti:
        // - Hvis absolut sti findes -> returnér den
        // - Hvis sti relativ til app base findes -> returnér fuld sti
        // - Ellers returnér en pack:// URI, som kan pege på resources i assembly
        private static string ResolveImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            var trimmed = path.Trim().Trim('"').Trim();

            // Normaliser skråstreger til platformens DirectorySeparatorChar
            var normalized = trimmed.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace('\\', System.IO.Path.DirectorySeparatorChar);

            // 1) Absolut sti
            if (System.IO.Path.IsPathRooted(trimmed) && File.Exists(trimmed))
                return trimmed;

            // 2) Prøv relativt til appens basefolder
            var appPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, normalized.TrimStart(System.IO.Path.DirectorySeparatorChar));
            if (File.Exists(appPath))
                return appPath;

            // 3) Returner en pack URI (bruges til embedded resources eller Content i assembly)
            var packPath = "pack://application:,,,/" + trimmed.Replace('\\', '/').TrimStart('/');
            return packPath;
        }

        // SplitLine: simple CSV-splitter der håndterer quoted felter og escaped quotes.
        private static IEnumerable<string> SplitLine(string line, char delimiter)
        {
            var cur = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inQuotes)
                {
                    // Hvis quote fundet inde i quote-blok, kan det være en escaped quote
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            // Tilføj enkelt quote og spring næste over
                            cur.Append('"');
                            i++;
                        }
                        else
                        {
                            // Slut på quoted blok
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        // Normalt tegn inde i quotes
                        cur.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        // Start quoted blok
                        inQuotes = true;
                    }
                    else if (c == delimiter)
                    {
                        // Separator: yield nuværende felt
                        yield return cur.ToString();
                        cur.Clear();
                    }
                    else
                    {
                        // Almindeligt tegn
                        cur.Append(c);
                    }
                }
            }
            // Returner sidste felt
            yield return cur.ToString();
        }

        // Fjern omgivelserende anførselstegn og af-escape dobbelte quotes.
        private static string Unquote(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length >= 2 && s[0] == '"' && s[^1] == '"')
                return s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            return s;
        }

        // Åbn detaljer for den valgte aktivitet i et popup-vindue
        private void OpenActivity(object sender, RoutedEventArgs e)
        {
            var selected = ActivityListBox.SelectedItem as ActivityClass;
            if (selected == null)
            {
                MessageBox.Show("Vælg en aktivitet først");
                return;
            }

            // Opret dialog og sæt ejer (så dialogcenterering/modale forhold virker korrekt)
            var dlg = new Popups.ActivityDetails(selected, "admin");
            dlg.Owner = Window.GetWindow(this);
            dlg.ShowDialog();
        }

        private void ActivityListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenActivity(sender, e);
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            // Naviger tilbage til login-siden
            NavigationService.Navigate(new Login());
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Placeholder hvis man vil håndtere valg af bruger
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = userList.SelectedItem as ActiviaAPP.Classes.User;

            if (selectedUser == null)
            {
                MessageBox.Show("Vælg et medlem først");
                return;
            }

            // Slet det markerede medlem fra UserStore
            UserStore.RegisteredUsers.Remove(selectedUser);
            MessageBox.Show($"Brugeren '{selectedUser.FullName}' er blevet slettet");
        }
    }
}

