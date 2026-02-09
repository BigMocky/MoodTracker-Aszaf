using System;

namespace MoodTracker.Model
{
    internal class Mood
    {
        public long Id { get; set; }                 // DB ID (ha van)
        public DateTime EntryDate { get; set; }      // nap
        public int MoodLevel { get; set; }           // 1-5
        public string Note { get; set; }             // megjegyzés
        public DateTime CreatedAt { get; set; }      // DB created_at (ha van)

        // 2 paraméteres konstruktor (követelmény)
        public Mood(DateTime entryDate, int moodLevel)
        {
            EntryDate = entryDate;
            MoodLevel = moodLevel;
            Note = "";
            CreatedAt = DateTime.Now;
        }

        // 3 paraméteres konstruktor (gyakoribb)
        public Mood(DateTime entryDate, int moodLevel, string note)
        {
            EntryDate = entryDate;
            MoodLevel = moodLevel;
            Note = note ?? "";
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{EntryDate:yyyy-MM-dd} | {MoodLevel} {Emoji(MoodLevel)} | {Note}";
        }

        public static string Emoji(int level)
        {
            switch (level)
            {
                case 1: return "😢";
                case 2: return "😕";
                case 3: return "😐";
                case 4: return "🙂";
                case 5: return "😄";
                default: return "❓";
            }
        }
    }
}
