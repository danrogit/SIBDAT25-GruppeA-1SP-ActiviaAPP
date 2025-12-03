using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ActiviaAPP.Classes
{
    public class ActivityClass
    {
        // Felter (private/backing) - kan bruges internt, men vi eksponerer properties nedenfor.
        public string activityTitle;
        public int groupSize;
        public DateTime date;
        public double activityPrice;

        // Public properties som bruges i UI og når aktiviteter oprettes
        public string ActivityTitle { get; set; } = string.Empty; // Titel på aktiviteten
        public string ActivityType { get; set; } = string.Empty;  // Type/kategori (f.eks. Sport, Natur)
        public string Description { get; set; } = string.Empty;   // Beskrivelse af aktiviteten
        public int MaxParticipants { get; set; }                  // Maks antal deltagere (0 = ingen begrænsning)
        public DateTime Date { get; set; }                        // Dato for aktiviteten
        public string CoverImagePath { get; set; } = string.Empty; // Sti eller pack URI til cover-billede

        // Beregnet property: antal nuværende tilmeldte deltagere
        public int CurrentParticipantCount => RegisteredUsers.Count;

        // Samling af bruger-ID'er (eller brugernavne) der er tilmeldt denne aktivitet.
        // ObservableCollection så UI automatisk opdateres når elementer ændres.
        public ObservableCollection<string> RegisteredUsers { get; } = new ObservableCollection<string>();
            
        // Metode: tilmeld en bruger til aktiviteten.
        // - Returnerer true hvis tilmelding lykkedes.
        // - Returnerer false hvis userID er ugyldigt, allerede tilmeldt, eller hvis aktiviteten er fuld.
        public bool Register(string userID)
        {
            // Valider input
            if (string.IsNullOrWhiteSpace(userID))
                return false;

            // Hvis brugeren allerede er tilmeldt -> afvis
            if (RegisteredUsers.Contains(userID))
                return false;

            // Hvis der er en max-begrænsning og vi allerede har nået den -> afvis
            if (MaxParticipants > 0 && RegisteredUsers.Count >= MaxParticipants)
                return false;

            // Tilføj brugeren til listen over tilmeldte
            RegisteredUsers.Add(userID);
            return true;
        }

        // Metode: afmeld en bruger fra aktiviteten.
        // - Returnerer false hvis userID er ugyldigt.
        // - Returnerer true hvis fjernelse lykkedes.
        public bool Unregister(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return false;

            // Remove returnerer true hvis elementet fandtes og blev fjernet
            return RegisteredUsers.Remove(userID);
        }

        // ToString overrider for bedre visning i lister
        public override string ToString()
        {
            // Viser titel og dato i formatet dd-MM-yyyy
            return $"{ActivityTitle} ({Date:dd-MM-yyyy})";
        }

        // Placeholder: metode til at oprette/initialisere aktiviteten yderligere hvis nødvendigt.
        public void CreateActivity()
        {
            // Implementer ekstra oprettelseslogik her hvis påkrævet.
        }
    }
}
