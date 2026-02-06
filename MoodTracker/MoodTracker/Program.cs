using MoodTracker.Model;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;

internal class Program
{
    private static List<List<string>> moodok = new List<List<string>>();
    private static List<Database> database = new List<Database>();
    private static FileIO.ReadFromFile reader = new FileIO.ReadFromFile();
    private static FileIO.WriteToFile writer = new FileIO.WriteToFile();

    private static void Main(string[] args)
    {
        Console.WriteLine("========MoodTracker========");
        Console.WriteLine("1 - Új hangulat rögzítése!");
        Console.WriteLine("2 - Előzmények megtekintése.");
        Console.WriteLine("3 - Átlag.");
        Console.WriteLine("0 - Kilépés!");

        AdatBeolvasas("moodtracker.sql", 4, ';', false);
        AdatBetoltes(moodok);
        
        Console.ReadLine();
        MelyikFunkcio();

        AdatDatabasebeIras();

        MelyikFunkcio();
        Console.ReadLine();

        Elozmenyek();
    }

    private static void Elozmenyek()
    {
        
    }

    private static void MelyikFunkcio()
    {
        if (Console.ReadLine() == "1")
        {
            Console.WriteLine("Új hangulat rögzítése!");
        }
        else if (Console.ReadLine() == "2")
        {
            Console.WriteLine("Előzmények megtekintése.");
        }
        else if (Console.ReadLine() == "3")
        {
            Console.WriteLine("Átlag.");
        }
        else if (Console.ReadLine() == "0")
        {
            Console.WriteLine("Kilépés!");
        }
    }

    private static void AdatDatabasebeIras()
    {
        List<string> fileData = new List<string>();
        foreach (var item in database)
        {
        if (item.Mood.ToString() == "1")
        {
            fileData.Add($"{item.Mood};{item.Atlag};{item.Datum};{item.Motivation}");
                Console.WriteLine("tortent valami");
        }
        else if (item.Mood.ToString() == "2")
        {
            fileData.Add($"{item.Mood};{item.Atlag};{item.Datum};{item.Motivation}");
        }
        else if (item.Mood.ToString() == "3")
        {
            fileData.Add($"{item.Mood};{item.Atlag};{item.Datum};{item.Motivation}");
        }
        else if (item.Mood.ToString() == "4")
        {
            fileData.Add($"{item.Mood};{item.Atlag};{item.Datum};{item.Motivation}");
        }
        else if (item.Mood.ToString() == "5")
        {
            fileData.Add($"{item.Mood};{item.Atlag};{item.Datum};{item.Motivation}");
        }
         
        }

        try
        {
            writer.FileWrite("moodtracker.sql", fileData);
            Console.WriteLine("Sikeres MOOD rögzítés!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);     
        }
        
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