using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MoodTracker.Model;

namespace MoodTracker
{
    internal class Program
    {
        private const string CsvPath = @"Document\mooddata.csv";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1) Betöltés külső CSV-ből (kötelező adatforrás)
            List<Mood> moods = LoadFromCsv(CsvPath);

            // 2) Betöltés adatbázisból (MySQL) – ha van kapcsolat, összevonjuk (duplikáció szűrés)
            Database db = new Database();
            if (db.CheckConnection())
            {
                List<Mood> dbMoods = db.SelectMoods();
                MergeWithoutDuplicates(moods, dbMoods);
            }

            // Induláskor biztos ami biztos: CSV frissítés az összevont listából
            SaveToCsv(CsvPath, moods);

            // Fő programciklus
            while (true) // while kötelező elem
            {
                DrawHeader("==== MoodTracker ====");
                Console.WriteLine("1 - Új hangulat rögzítése (CSV + DB)");
                Console.WriteLine("2 - Előzmények megtekintése");
                Console.WriteLine("3 - Statisztika");
                Console.WriteLine("4 - Keresés megjegyzésben");
                Console.WriteLine("0 - Kilépés");
                Console.Write("\n> ");

                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice) // switch-case kötelező elem
                {
                    case "1":
                        AddMoodFlow(moods, db);
                        break;
                    case "2":
                        ShowHistoryFlow(moods);
                        break;
                    case "3":
                        ShowStatsFlow(moods);
                        break;
                    case "4":
                        SearchFlow(moods);
                        break;
                    case "0":
                        // Kilépés előtt mentjük CSV-be (biztonság)
                        SaveToCsv(CsvPath, moods);
                        Info("Viszlát! (CSV mentés kész.)");
                        return;
                    default:
                        Warn("Ismeretlen menüpont!");
                        Pause();
                        break;
                }
            }
        }

        // ---------------------------
        //  MENÜ FUNKCIÓK
        // ---------------------------

        private static void AddMoodFlow(List<Mood> moods, Database db)
        {
            DrawHeader("Új hangulat rögzítése");

            DateTime entryDate = AskDate("Dátum (yyyy-MM-dd) [Enter = ma]: ", DateTime.Today);

            int moodLevel = AskIntInRange("Hangulat (1-5): ", 1, 5);
            Console.Write("Megjegyzés (rövid): ");
            string note = Console.ReadLine() ?? "";

            Mood mood = new Mood(entryDate, moodLevel, note);

            // Listába tesszük
            moods.Add(mood);

            // ALAPÉRTELMEZETT mentés CSV-be (minden új bejegyzés után)
            SaveToCsv(CsvPath, moods);
            Success("Mentve CSV-be ✔");

            // ALAPÉRTELMEZETT mentés DB-be is (ha van kapcsolat)
            if (db.CheckConnection())
            {
                bool ok = db.InsertMood(mood);
                if (ok) Success("Mentve adatbázisba is ✔");
                else Warn("DB mentés nem sikerült (de CSV + lista frissült).");
            }
            else
            {
                Warn("Nincs DB kapcsolat – csak CSV/lista frissült.");
            }

            // Motiváló üzenet
            Console.WriteLine();
            Console.WriteLine("💬 " + GetRandomMessage(moodLevel));

            Pause();
        }

        private static void ShowHistoryFlow(List<Mood> moods)
        {
            DrawHeader("Előzmények");

            if (moods.Count == 0)
            {
                Warn("Még nincs bejegyzés.");
                Pause();
                return;
            }

            // rendezés dátum szerint
            moods.Sort((a, b) => a.EntryDate.CompareTo(b.EntryDate));

            PrintTableHeader();
            foreach (Mood m in moods) // foreach kötelező elem
            {
                PrintRow(m);
            }
            PrintTableFooter();

            // Extra: napi szűrés (kiválasztás)
            Console.WriteLine();
            DateTime day = AskDate("Szűrés egy napra? (yyyy-MM-dd) [Enter = kihagy]: ", DateTime.MinValue, allowEmpty: true);
            if (day != DateTime.MinValue)
            {
                List<Mood> daily = new List<Mood>();
                foreach (var m in moods)
                {
                    if (m.EntryDate.Date == day.Date) daily.Add(m);
                }

                Console.WriteLine();
                if (daily.Count == 0) Warn("Nincs bejegyzés erre a napra.");
                else
                {
                    Info($"Találat: {daily.Count} db");
                    PrintTableHeader();
                    for (int i = 0; i < daily.Count; i++) // for ciklus kötelező elem
                        PrintRow(daily[i]);
                    PrintTableFooter();
                }
            }

            Pause();
        }

        private static void ShowStatsFlow(List<Mood> moods)
        {
            DrawHeader("Statisztika");

            if (moods.Count == 0)
            {
                Warn("Még nincs adat.");
                Pause();
                return;
            }

            // Összegzés
            int sum = 0;
            foreach (var m in moods) sum += m.MoodLevel;
            double avg = (double)sum / moods.Count;

            // Kiválasztás: legjobb/legrosszabb nap
            Mood best = moods[0];
            Mood worst = moods[0];
            foreach (var m in moods)
            {
                if (m.MoodLevel > best.MoodLevel) best = m;
                if (m.MoodLevel < worst.MoodLevel) worst = m;
            }

            // Gyakoriságok Dictionary-vel
            Dictionary<int, int> counts = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++) counts[i] = 0;
            foreach (var m in moods) counts[m.MoodLevel]++;

            // Kiírás formázva
            Console.WriteLine($"Bejegyzések száma: {moods.Count:N0}");
            Console.WriteLine($"Átlag hangulat:     {avg:0.00} / 5.00");
            Console.WriteLine($"Legjobb nap:        {best.EntryDate:yyyy-MM-dd} ({best.MoodLevel} {Mood.Emoji(best.MoodLevel)})");
            Console.WriteLine($"Legrosszabb nap:    {worst.EntryDate:yyyy-MM-dd} ({worst.MoodLevel} {Mood.Emoji(worst.MoodLevel)})");
            Console.WriteLine();

            Console.WriteLine("Eloszlás (1-5):");
            for (int level = 1; level <= 5; level++)
            {
                Console.WriteLine($"  {level} {Mood.Emoji(level)} : {counts[level],3} db");
            }

            Console.WriteLine();

            // Ternary operátor (kötelező elem)
            string msg = avg >= 3.8 ? "Szép időszak 😊"
                      : avg >= 2.8 ? "Vegyes napok – tartsd a ritmust 💪"
                      : "Tarts szünetet 💙";

            if (avg >= 3.8) Success(msg);
            else if (avg >= 2.8) Info(msg);
            else Warn(msg);

            // Függvény out + ref (követelmény)
            int total;
            double average;
            CalcSumAndAvg(moods, out total, out average);
            Console.WriteLine($"\n(out ellenőrzés) Összeg: {total}, Átlag: {average:0.00}");

            Pause();
        }

        private static void SearchFlow(List<Mood> moods)
        {
            DrawHeader("Keresés megjegyzésben");

            Console.Write("Keresett szó/részlet: ");
            string q = (Console.ReadLine() ?? "").Trim();

            if (q.Length == 0)
            {
                Warn("Üres keresés.");
                Pause();
                return;
            }

            // Keresés
            List<Mood> hits = new List<Mood>();
            foreach (var m in moods)
            {
                if ((m.Note ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                    hits.Add(m);
            }

            Console.WriteLine();
            if (hits.Count == 0)
            {
                Warn("Nincs találat.");
            }
            else
            {
                Info($"Találat: {hits.Count} db");
                PrintTableHeader();
                foreach (var m in hits) PrintRow(m);
                PrintTableFooter();
            }

            Pause();
        }

        // ---------------------------
        //  ADATKEZELÉS (CSV)
        // ---------------------------

        private static List<Mood> LoadFromCsv(string path)
        {
            List<Mood> moods = new List<Mood>();

            if (!File.Exists(path))
            {
                // alap fájl létrehozása
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
                File.WriteAllText(path, "entry_date;mood_level;note\n");
                return moods;
            }

            string[] lines = File.ReadAllLines(path);

            // 0. sor a fejléc
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0) continue;

                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;

                if (!DateTime.TryParse(parts[0], out DateTime date)) continue;
                if (!int.TryParse(parts[1], out int level)) continue;

                string note = parts.Length >= 3 ? parts[2] : "";
                moods.Add(new Mood(date, level, note));
            }

            return moods;
        }

        private static void SaveToCsv(string path, List<Mood> moods)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine("entry_date;mood_level;note");
                foreach (var m in moods)
                {
                    sw.WriteLine($"{m.EntryDate:yyyy-MM-dd};{m.MoodLevel};{EscapeCsv(m.Note)}");
                }
            }
        }

        private static string EscapeCsv(string s)
        {
            s = s ?? "";
            return s.Replace(';', ',');
        }

        // ---------------------------
        //  SEGÉDFÜGGVÉNYEK
        // ---------------------------

        private static void MergeWithoutDuplicates(List<Mood> target, List<Mood> add)
        {
            foreach (var m in add)
            {
                bool exists = false;
                foreach (var t in target)
                {
                    if (t.EntryDate.Date == m.EntryDate.Date &&
                        t.MoodLevel == m.MoodLevel &&
                        (t.Note ?? "") == (m.Note ?? ""))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists) target.Add(m);
            }
        }

        private static void CalcSumAndAvg(List<Mood> moods, out int sum, out double avg)
        {
            sum = 0;
            foreach (var m in moods) sum += m.MoodLevel;
            avg = moods.Count == 0 ? 0 : (double)sum / moods.Count;
        }

        private static int AskIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
                Warn($"Hibás érték! ({min}-{max})");
            }
        }

        private static DateTime AskDate(string prompt, DateTime defaultValue, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();

                if (s.Length == 0)
                {
                    if (allowEmpty) return DateTime.MinValue;
                    return defaultValue;
                }

                if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime dt))
                    return dt;

                Warn("Hibás dátum! Formátum: yyyy-MM-dd");
            }
        }

        private static string GetRandomMessage(int moodLevel)
        {
            // ternary operátor is szerepel a programban, itt is megmaradhat
            string tone = moodLevel >= 4 ? "pozitív" : (moodLevel >= 3 ? "semleges" : "támogató");

            string[] positive =
            {
                "Ma nagyon jól tolod! 😄",
                "Ez az! Légy büszke magadra! ✨",
                "Szuper nap – tartsd meg ezt az érzést! 🌟"
            };

            string[] neutral =
            {
                "Haladsz, lépésről lépésre. 🙂",
                "Jó, hogy figyelsz magadra. 🧠",
                "Egy átlagos nap is értékes. 🌿"
            };

            string[] supportive =
            {
                "Most legyél kedves magaddal. 💙",
                "Kis pihenő is számít. 🫶",
                "Ha kell, beszélj valakivel – nem vagy egyedül. 🤝"
            };

            Random rnd = new Random();
            if (tone == "pozitív") return positive[rnd.Next(positive.Length)];
            if (tone == "semleges") return neutral[rnd.Next(neutral.Length)];
            return supportive[rnd.Next(supportive.Length)];
        }

        // ---------------------------
        //  KONZOLOS “CSINOSÍTÁS”
        // ---------------------------

        private static void DrawHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(title);
            Console.ResetColor();
            Console.WriteLine(new string('=', title.Length));
            Console.WriteLine();
        }

        private static void PrintTableHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+------------+------+-------+------------------------------------------+");
            Console.WriteLine("| Dátum       | Mood | Emoji | Megjegyzés                                |");
            Console.WriteLine("+------------+------+-------+------------------------------------------+");
            Console.ResetColor();
        }

        private static void PrintRow(Mood m)
        {
            string note = (m.Note ?? "");
            if (note.Length > 42) note = note.Substring(0, 39) + "...";

            Console.WriteLine($"| {m.EntryDate:yyyy-MM-dd} |  {m.MoodLevel}   |  {Mood.Emoji(m.MoodLevel),-3} | {note,-42} |");
        }

        private static void PrintTableFooter()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+------------+------+-------+------------------------------------------+");
            Console.ResetColor();
        }

        private static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Pause()
        {
            Console.WriteLine("\nNyomj Entert a folytatáshoz...");
            Console.ReadLine();
        }
    }
}
