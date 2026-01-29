using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTracker.Model
{
    internal class Database
    {
        private int _Mood;
        private double _Atlag;
        private string _Datum;
        private string _Motivation;

        public Database()
        {

        }

        public Database(int mood, double atlag, string datum, string motivation)
        {
            Mood = mood;
            Atlag = atlag;
            Datum = datum;
            Motivation = motivation;
        }

        public int Mood { get => _Mood; set => _Mood = value; }
        public double Atlag { get => _Atlag; set => _Atlag = value; }
        public string Datum { get => _Datum; set => _Datum = value; }
        public string Motivation { get => _Motivation; set => _Motivation = value; }
    }
}
