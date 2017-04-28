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
            Clubs clubs = new Clubs("ky", 41014);

            foreach (string name in clubs.ClubsIDs)
                Console.WriteLine(name);
            Console.WriteLine("");
        }
    }
}
