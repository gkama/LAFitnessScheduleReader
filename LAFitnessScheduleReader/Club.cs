using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFitnessScheduleReader
{
    class Club
    {
        public string State { get; set; }
        public string Zip { get; set; }
        public string ClubURI { get; set; }
        public string ClubName { get; set; }
        public string ClubAddress { get; set; }
        public string ClubDescription { get; set; }
        public string ClubID { get; set; }

        public Club(string State, string Zip, string ClubURI, string ClubName, string ClubAddress, string ClubDescription, string ClubID)
        {
            this.State = State;
            this.Zip = Zip;
            this.ClubURI = ClubURI;
            this.ClubName = ClubName;
            this.ClubAddress = ClubAddress;
            this.ClubDescription = ClubDescription;
            this.ClubID = ClubID;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.ClubName, ClubAddress);
        }

        //Output functions
        public string GetClassSchedule(string Day = null, string Time = null)
        {
            if (Day == null & Time == null)
                return "";
            else if (Day != null & Time == null)
            {
                Classes classes = new Classes(this.ClubID);
                return classes.GetClassesByDay(Day);
            }
            else if (Day == null && Time != null)
            {
                Classes classes = new Classes(this.ClubID);
                return classes.GetClassesByTime(Time);
            }
            else
            {
                Classes classes = new Classes(this.ClubID);
                return classes.GetClassesByDayTime(Day, Time);
            }         
        }
    }
}
