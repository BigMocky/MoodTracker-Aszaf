using MoodTracker.Model;
using System.Diagnostics;
using System.Security.AccessControl;

internal class Program
{
    private static List<List<string>> moodok = new List<List<string>>();
    private static List<Database> database = new List<Database>();
    private static FileRead.ReadFromFile reader = new FileRead.ReadFromFile();
//    private static FileRead.WriteToFile writer = new FileRead.WriteToFile();

    private static void Main(string[] args)
    {
        AdatBeolvasas("moodtracker.sql", 4, ';', false);
        AdatBetoltes(moodok);
        AdatDatabasebeIras();
    }

    private static void AdatDatabasebeIras()
    {
        /*
               List<string> fileData = new List<string>();
               foreach (var item in moodok)
               {
                   if (item.Mood.Substring(0, 1).ToLower() == "a")
                   {
                       fileData.Add($"{item.Mood};{item.Datum}");
                   }
               }

               try
               {
                   writer.FileWrite("moodtracker.sql", fileData);
               }
               catch (Exception)
               {

                   throw;
               }
        */
    }

    private static void AdatBetoltes(List<List<string>> moodok)
    {
        foreach (var item in moodok)
        {
            Database m = new Database();
            m.Mood = Convert.ToInt32(item[0]);
            m.Atlag = Convert.ToDouble(item[1]);
            m.Datum = item[2];
            m.Motivation = item[3];

            database.Add(m);
        }
    }

    private static void AdatBeolvasas(string v1, int v2, char v3, bool v4)
    {
        moodok = reader.FileRead(v1, v2, v3, v4);
    }
}