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
            Classes _classes = new Classes("986&Bellevue-Kentucky");
            Classes schedule = new Classes("465&Cincinnati-Ohio");
            Console.WriteLine(_classes.GetClassesByTime("5:45 am"));
            Console.WriteLine("\r\n");
            Console.WriteLine(schedule.GetClassesByTime("5:45 am"));
            Console.WriteLine(schedule.GetClassDescription(_classes.ClassesList[34].ClassDescription));
        }
    }
}
