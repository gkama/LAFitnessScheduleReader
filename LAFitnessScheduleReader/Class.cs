using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFitnessScheduleReader
{
    public class Class
    {
        public string Time { get; set; }
        public string Day { get; set; }
        public string ClassName { get; set; }
        public string ClassDescriptionsURI { get; set; }
        public string ClassDescription { get; set; }       

        public Class(string Time, string Day, string ClassName, string ClassDescriptionsURI, string ClassDescription)
        {
            this.Time = Time;
            this.Day = Day;
            this.ClassName = ClassName;
            this.ClassDescriptionsURI = ClassDescriptionsURI;
            this.ClassDescription = ClassDescription;
        }

        //Override to string method
        public override string ToString()
        {
            return string.Format("{0}: {1} - {2}", this.Time, this.Day, this.ClassName);
        }
    }
}
