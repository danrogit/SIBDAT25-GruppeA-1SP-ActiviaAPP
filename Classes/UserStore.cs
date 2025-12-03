using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiviaAPP.Classes
{
    public class UserStore
    {
        // Den delte liste for alle vores registrerede brugere.
        // ObservableCollection bruges så UI automatisk kan opdatere ved ændringer.
        public static ObservableCollection<User> RegisteredUsers { get; } = new ObservableCollection<User>();

        // Finder en bruger ud fra brugernavn og password.
        // Returnerer null hvis ingen match findes.
        public static User? FindUser(string username, string password)
        {
            return RegisteredUsers.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }

        // Tjekker om et brugernavn allerede er registreret (case-insensitivt).
        public static bool UsernameExists(string username)
        {
            return RegisteredUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // Registrerer en ny bruger i RegisteredUsers.
        // Returnerer false hvis brugernavnet allerede findes.
        public static bool RegisterUser(User user)
        {
            if (UsernameExists(user.Username))
                return false;

            RegisteredUsers.Add(user);
            return true;
        }

        // Indlæs brugere fra en CSV-fil.
        // Simpel UTF-8 reader (ingen encoding-fallbacks).
        public static void LoadFromCsv(string filePath)
        {
            // Input-validering: sørg for at filsti er gyldig
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath is null or empty", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV file not found", filePath);

            // Læs alle linjer som UTF-8 (forvent standard CSV-format)
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // Ryd eksisterende liste før indlæsning
            RegisteredUsers.Clear();

            foreach (var rawLine in lines)
            {
                // Spring tomme linjer over
                if (string.IsNullOrWhiteSpace(rawLine))
                    continue;

                // Trim og fjern evt. omgivende parenteser: "(...)" -> "..."
                var raw = rawLine.Trim();
                if (raw.StartsWith("(") && raw.EndsWith(")"))
                    raw = raw.Substring(1, raw.Length - 2).Trim();

                // Vælg delimiter: foretræk semikolon, ellers komma
                char delimiter = raw.Contains(';') ? ';' : ',';

                // Parse linjen i felter; ParseDelimitedLine håndterer quoted fields og escaped quotes
                var fields = ParseDelimitedLine(raw, delimiter)
                             .Select(f => Unquote(f.Trim()))
                             .ToArray();

                if (fields.Length == 0)
                    continue;

                    // Spring header-rækker over hvis første kolonne ligner en header
                var first = fields[0].Trim();
                if (first.Equals("username", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("brugernavn", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("fullname", StringComparison.OrdinalIgnoreCase) ||
                    first.Equals("navn", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Map felter til User-objekt.
                // Vi understøtter to almindelige ordener:
                // - username,password,fullname,email,phone
                // - fullname;username;password;email;phone
                string username = string.Empty;
                string password = string.Empty;
                string fullname = string.Empty;
                string email = string.Empty;
                string phone = string.Empty;

                // Hvis første felt indeholder mellemrum og ikke ligner en e-mail, antages det at være fullname
                if (fields.Length >= 5 && fields[0].Contains(' ') && !fields[0].Contains("@"))
                {
                    // Sandsynlig format: FullName;Username;Password;Email;Phone
                    fullname = fields[0];
                    username = fields[1];
                    password = fields[2];
                    email = fields[3];
                    phone = fields[4];
                }
                else if (fields.Length >= 5)
                {
                    // Antag format: username,password,fullname,email,phone
                    username = fields[0];
                    password = fields[1];
                    fullname = fields[2];
                    email = fields[3];
                    phone = fields[4];
                }
                else
                {
                    // Fallback: udfyld hvad vi kan fra de tilgængelige felter
                    if (fields.Length > 0) username = fields[0];
                    if (fields.Length > 1) password = fields[1];
                    if (fields.Length > 2) fullname = fields[2];
                    if (fields.Length > 3) email = fields[3];
                    if (fields.Length > 4) phone = fields[4];
                }

                var user = new User
                {
                    Username = username ?? string.Empty,
                    Password = password ?? string.Empty,
                    FullName = fullname ?? string.Empty,
                    Email = email ?? string.Empty,
                    Phone = phone ?? string.Empty
                };

                // Hvis brugernavn er tomt, spring linjen over
                if (string.IsNullOrWhiteSpace(user.Username))
                    continue;

                // Undgå duplikater: tilføj kun hvis brugernavnet ikke allerede er registreret
                if (!UsernameExists(user.Username))
                    RegisteredUsers.Add(user);
            }
        }

        // Minimal parser for delimited linje.
        // Understøtter quoted fields med separator indeni og escaped quotes ("").
        private static string[] ParseDelimitedLine(string line, char delimiter)
        {
            var result = new List<string>();
            if (line == null)
                return result.ToArray();

            var cur = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Hvis næste tegn også er en quote => escaped quote -> tilføj en enkelt quote
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            cur.Append('"');
                            i++; // spring den ekstra quote over
                        }
                        else
                        {
                            // Afslutning af quoted felt
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        // Inden i quotes: tilføj tegn som det er
                        cur.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        // Start af quoted felt
                        inQuotes = true;
                    }
                    else if (c == delimiter)
                    {
                        // Separator: færdiggør nuværende felt
                        result.Add(cur.ToString());
                        cur.Clear();
                    }
                    else
                    {
                        // Almindeligt tegn i feltet
                        cur.Append(c);
                    }
                }
            }
            // Tilføj sidste felt (kan være tomt)
            result.Add(cur.ToString());
            return result.ToArray();
        }

        // Fjern omgivelserende anførselstegn hvis til stede og af-escape dobbelte quotes.
        private static string Unquote(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (s.Length >= 2 && s[0] == '"' && s[^1] == '"')
                return s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            return s;
        }
    }
}
