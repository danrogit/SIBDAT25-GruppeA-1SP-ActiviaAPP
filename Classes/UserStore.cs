using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiviaAPP.Classes
{
    //Klasse til håndtering af brugere når disse oprettes
    public class UserStore
    {
        //ObservableCollection til håndtering af medlemmer (denne listetype er redigerbar)
        public static ObservableCollection<User> RegisteredUsers { get; } = new ObservableCollection<User>();
      
        //Metode til login af "user"
        public static User? FindUser(string username, string password)
        {
            //Søger gennem listen "RegisteredUsers", og returnerer den bruger der matcher brugernavn og kodeord, ellers returnere den null
            return RegisteredUsers.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }
     
        //Metode til at tjekke om et brugernavn allerede eksisterer i listen "RegisteredUsers", ved oprettelse af login
        public static bool UsernameExists(string username)
        {
            return RegisteredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
       
        //Metode til at oprette et nyt medlem, hvis brugernavnet ikke allerede eksisterer
        public static bool RegisterUser(User user)
        {
            //Hvis det angivne brugernavn allerede eksisterer, returneres false
            if (UsernameExists(user.Username))
                return false;

            //Hvis brugernavnet ikke eksisterer, tilføjes brugeren til listen og returneres true
            RegisteredUsers.Add(user);
            return true;
        }
    
        //Medtode til indlæsning af brugere fra en CSV fil
        public static void LoadFromCsv(string filePath)
        {
            //Der tjekkes for gyldig filsti og om filen eksisterer, ellers kommes der med en fejlmeddelelse
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Filsti eksisterer ikke", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV fil ikke fundet", filePath);

            //Filen læses linje for linje, og gemmes i en array af strings
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            //Rydder eksisterende liste før indlæsning af filen
            RegisteredUsers.Clear();

            //Loop til at gennemgå hver linje i filen
            foreach (var rawLine in lines)
            {
                //Springer tomme linjer over i filen
                if (string.IsNullOrWhiteSpace(rawLine))
                    continue;

                //Fjerner udnødvendige mellemrum, "" og parenteser
                var raw = rawLine.Trim();
                if (raw.StartsWith("(") && raw.EndsWith(")"))
                    raw = raw.Substring(1, raw.Length - 2).Trim();

                //Kode der "delimiter" - adskiller felterne i linjen ud fra enten semikolon eller komma
                char delimiter = raw.Contains(';') ? ';' : ',';

                //Deler filen op i felter, fjerner unødvendige mellemrum og "", og gemmer dem i et array
                var fields = ParseDelimitedLine(raw, delimiter)
                             .Select(f => Unquote(f.Trim()))
                             .ToArray();

                //Springer linjen over hvis der ikke er nogen felter
                if (fields.Length == 0)
                    continue;

                //Fjerner eventuelle tomme tegn i starten af det første felt
                var first = fields[0].Trim();

                //Sammenligner det første felt med kendte header-navne og springer linjen over hvis der er et match
                if (first.Equals("username", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("brugernavn", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("fullname", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("navn", StringComparison.OrdinalIgnoreCase))
                    continue;

                //Variabler til at holde brugeroplysninger af medlemmerne
                string username = string.Empty;
                string password = string.Empty;
                string fullname = string.Empty;
                string email = string.Empty;
                string phone = string.Empty;
              
                //Hvis det første felt ikke indeholder "@", antages det at være "fullname"
                if (fields.Length >= 5 && fields[0].Contains(' ') && !fields[0].Contains("@"))
                {
                    //Det antages at formatet i filen er: fullname, username, password, email, phone
                    fullname = fields[0];
                    username = fields[1];
                    password = fields[2];
                    email = fields[3];
                    phone = fields[4];
                }

                //Hvis ikke at ovenstående er tilfældet, antages det at være i formatet: username, password, fullname, email, phone
                else if (fields.Length >= 5)
                {
                    //Det antages at formatet i filen er: fullname, username, password, email, phone
                    username = fields[0];
                    password = fields[1];
                    fullname = fields[2];
                    email = fields[3];
                    phone = fields[4];
                }

                //Hvis ikke de ovenståede matcher, håndteres det som et "fallback"
                else
                {
                    //Der udfyldes hvad der er tilgængeligt i felterne
                    if (fields.Length > 0) username = fields[0];
                    if (fields.Length > 1) password = fields[1];
                    if (fields.Length > 2) fullname = fields[2];
                    if (fields.Length > 3) email = fields[3];
                    if (fields.Length > 4) phone = fields[4];
                }

                //Opretter et nyt medlemsobjekt ud fra de tastede oplysninger
                var user = new User
                {
                    //Sætter værdierne til det valgte af medlemmet, ellers er værdien en tom string, dette sikrer mod null værdier
                    Username = username ?? string.Empty,
                    Password = password ?? string.Empty,
                    FullName = fullname ?? string.Empty,
                    Email = email ?? string.Empty,
                    Phone = phone ?? string.Empty
                };

                //Hvis brugernavnet er tomt, springes linjen over
                if (string.IsNullOrWhiteSpace(user.Username))
                    continue;

                //Hvis ikke brugernavnet allerede eksisterer, tilføjes medlemmet til listen
                if (!UsernameExists(user.Username))

                    //Brugernavnet/medlemmet tilføjes til RegisteredUsers listen
                    RegisteredUsers.Add(user);
            }
        }

        //Metode til at køre en linje igennem og splitte den op i felter baseret på en given delimiter/separator
        private static string[] ParseDelimitedLine(string line, char delimiter)
        {
            //Liste til at holde de opdelte felter
            var result = new List<string>();

            //Hvis linjen er null, returneres et tom array
            if (line == null)
                return result.ToArray();

            //StringBuilder benyttes til at have en redigerbar string
            var cur = new StringBuilder();

            //Bool til at holde styr på om vi er inden i anførselstegn
            bool inQuotes = false;

            //Loop der kører gennem hvert tegn i linjen
            for (int i = 0; i < line.Length; i++)
            {
                //Henter det aktuelle tegn
                char c = line[i];

                //Så længe at vi er inden i anførselstegn, er den "true"
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        
                        //Deler ordene hvis der er dobbelte anførselstegn
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            cur.Append('"');
                            i++;
                        }
                        else
                        {
                            //Afslutning af quoted felt
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        //Tilføjer tegn som det er
                        cur.Append(c);
                    }
                }
                else
                {
                    //Sørger for, at behandle hele teksten korrekt, medtager derfor , og andre tegn
                    if (c == '"')
                    {
                        //
                        inQuotes = true;
                    }

                    //Hvis der er en delimiter, afsluttes feltet
                    else if (c == delimiter)
                    {
                        //Resultatet gemmes i "result" listen
                        result.Add(cur.ToString());

                        //"cur" nulstilles for næste felt
                        cur.Clear();
                    }

                    //Tegnet tilføjes til det aktuelle felt
                    else
                    {                      
                        cur.Append(c);
                    }
                }
            }

            //Tilføjer sidste felt også selvom det er tomt
            result.Add(cur.ToString());
            return result.ToArray();
        }

        //Funktion der fjerner eventuelle omgivende anførselstegn
        private static string Unquote(string s)
        {
            //Hvis strengen er tom eller null, returneres den som den er
            if (string.IsNullOrEmpty(s))
                return s;

            //Hvis strengen starter og slutter med anførselstegn, fjernes de og eventuelle dobbelte "" erstattes med "
            if (s.Length >= 2 && s[0] == '"' && s[^1] == '"')
                return s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            return s;
        }
    }
}
