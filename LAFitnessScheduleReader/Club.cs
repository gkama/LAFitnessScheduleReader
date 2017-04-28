using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFitnessScheduleReader
{
    internal class Club
    {
        public string State { get; set; }
        public string Zip { get; set; }
        public string ClubURI { get; set; }
        public string ClubName { get; set; }
        public string ClubAddress { get; set; }
        public string ClubDescription { get; set; }
        public string ClubID { get; set; }
        public Classes Classes { get; set; }
        public List<string> ClassesList { get; set; }

        public Club(string State, string Zip, string ClubURI, string ClubName, string ClubAddress, string ClubDescription, string ClubID, 
            Classes Classes, List<string> ClassesList)
        {
            this.State = State;
            this.Zip = Zip;
            this.ClubURI = ClubURI;
            this.ClubName = ClubName;
            this.ClubAddress = ClubAddress;
            this.ClubDescription = ClubDescription;
            this.ClubID = ClubID;
            this.Classes = Classes;
            this.ClassesList = ClassesList;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.ClubName, this.ClubAddress);
        }

        //Output functions
        public string GetClassSchedule(string Day = null, string Time = null)
        {
            if (Day == null && Time == null)
            {
                return this.Classes.GetClasses();
            }
            else if (Day != null && Time == null)
            {
                return this.Classes.GetClassesByDay(Day);
            }
            else if (Day == null && Time != null)
            {
                return this.Classes.GetClassesByTime(Time);
            }
            else
            {
                return this.Classes.GetClassesByDayTime(Day, Time);
            }         
        }
        public string GetClassSchedule()
        {
            StringBuilder toReturn = new StringBuilder();
            foreach (string s in this.ClassesList)
            {
                toReturn.Append(s).Append("\r\n");
            }
            return toReturn.ToString();
        }
    }
}
