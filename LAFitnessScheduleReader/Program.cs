using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LAFitnessScheduleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Classes _classes = new Classes("986&Bellevue-Kentucky");
            //Classes schedule = new Classes("465&Cincinnati-Ohio");
            Clubs clubs = new Clubs("ky", 41005);

            //foreach (string name in clubs.ClubsNames)
                Console.WriteLine(clubs.GetClassSchedules("ky", 41005, null, "6:00 pm"));
        }
    }
}
