using System;
using System.Net;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LAFitnessScheduleReader
{
    public class Classes
    {
        //Public
        public List<Class> ClassesList { get; set; }
        public string ClubID { get; set; }
        public Dictionary<string, List<string>> ClassesByDay { get; set; }
        public Dictionary<string, List<string>> ClassesByTime { get; set; }
        public Dictionary<string, string> ClassesDescriptions { get; set; }
        public List<string> Times { get; set; }
        public List<string> Days = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        //Private
        private string URL { get; set; }
        private List<string> ClassDescriptionsURIs { get; set; }

        //Constructor
        public Classes(string ClubID)
        {
            try
            {
                this.ClubID = ClubID;
                ClassesList = new List<Class>();
                ClassesByDay = new Dictionary<string, List<string>>();
                ClassesByTime = new Dictionary<string, List<string>>();
                ClassesDescriptions = new Dictionary<string, string>();
                ClassDescriptionsURIs = new List<string>();
                Times = new List<string>();

                GetData();
                SetClassesByDay();
                SetClassesByTime();
                SetClassDescriptions();
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }


        //Get the Schedule list of classes
        public string Schedule()
        {
            try
            {
                StringBuilder toReturn = new StringBuilder();
                foreach (Class c in this.ClassesList)
                {
                    toReturn.Append(c.ToString()).Append("\r\n");
                }
                return toReturn.ToString().TrimEnd('\n').TrimEnd('\r');
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        //Get classes by day
        public string GetClassesByDay(string Day)
        {
            try
            {
                Day = char.ToUpper(Day[0]) + Day.Substring(1);
                StringBuilder toReturn = new StringBuilder();

                foreach (string s in ClassesByDay[Day])
                    toReturn.Append(s).Append("\r\n");
                return toReturn.ToString().TrimEnd('\n').TrimEnd('\r');
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Get classes by time
        public string GetClassesByTime(string Time)
        {
            try
            {
                StringBuilder toReturn = new StringBuilder();
                DateTime dt = Convert.ToDateTime(Time);
                Time = dt.ToString("hh:mm tt").ToUpper();

                foreach (string s in ClassesByTime[Time])
                        toReturn.Append(s).Append("\r\n");
                return toReturn.ToString().TrimEnd('\n').TrimEnd('\r');
            }
            catch (FormatException) { throw new FormatException("Incorrect Format for Time"); }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Get classes by date and time
        public string GetClassesByDayTime(string Day, string Time)
        {
            try
            {
                StringBuilder toReturn = new StringBuilder();
                Day = char.ToUpper(Day[0]) + Day.Substring(1);
                DateTime dt = Convert.ToDateTime(Time);
                Time = dt.ToString("hh:mm tt").ToUpper();

                foreach (Class c in this.ClassesList)
                    if (c.Time.Trim().ToUpper().Equals(Time) && c.Day.Equals(Day))
                        toReturn.Append(c.ToString()).Append("\r\n");
                return toReturn.ToString().TrimEnd('\n').TrimEnd('\r');
            }
            catch (FormatException) { throw new FormatException("Incorrect Format for Time"); }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Get classes descriptions
        public string GetClassDescription(string ClassName)
        {
            ClassName = ClassName.ToUpper();
            if (ClassesDescriptions.ContainsKey(ClassName))
                return ClassesDescriptions[ClassName];
            else
                return "Not Available";
        }


        //Set the classes by day
        private void SetClassesByDay()
        {
            try
            {
                foreach (string day in Days)
                {
                    List<string> _classes = new List<string>();
                    foreach (Class c in ClassesList)
                    {
                        if (c.Day.Equals(day))
                            _classes.Add(c.ToString());
                    }
                    ClassesByDay.Add(day, _classes);
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Set the classes by time
        private void SetClassesByTime()
        {
            try
            {
                //Get all unique times
                foreach (string time in Times)
                {
                    List<string> _classes = new List<string>();
                    foreach (Class c in ClassesList)
                    {
                        if (c.Time.Equals(time))
                            _classes.Add(c.ToString());
                    }
                    ClassesByTime.Add(time, _classes);
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Set the descriptions
        private void SetClassDescriptions()
        {
            try
            {
                foreach (string uri in ClassDescriptionsURIs)
                {
                    StringBuilder toReturn = new StringBuilder();
                    string DescURL = ConfigurationManager.AppSettings["LAFitness"] + uri;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(ReadData(DescURL));

                    string ClassTitle = doc.GetElementbyId("ctl00_MainContent_rptClasses_ctl00_lblTitle").InnerText;
                    string ClassDesc = doc.GetElementbyId("ctl00_MainContent_rptClasses_ctl00_lblDescription").InnerText;

                    if (!ClassesDescriptions.ContainsKey(ClassTitle))
                        ClassesDescriptions.Add(ClassTitle.ToUpper(), ClassDesc);
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        //Get schedule
        private void GetData()
        {
            try
            {
                var doc = new HtmlDocument();
                URL = ConfigurationManager.AppSettings["BaseURL"] + this.ClubID;
                doc.LoadHtml(ReadData(URL));

                var table = doc.GetElementbyId("tblSchedule");

                foreach (var tr in table.SelectSingleNode("tbody").SelectNodes("tr"))
                {
                    string time = tr.SelectSingleNode("th").SelectSingleNode("h5").InnerText;
                    string className = string.Empty;
                    string classDescriptionuri = string.Empty;
                    string classDescription = string.Empty;
                    int currDayInt = 0;
                    foreach (var td in tr.SelectNodes("td"))
                    {
                        string currDay = Days[currDayInt];
                        if (td.HasChildNodes)
                        {
                            className = td.InnerText;
                            classDescription = className.Split('(')[0].Trim();
                            try
                            {
                                classDescriptionuri = td.SelectSingleNode(".//strong//a").GetAttributeValue("href", "Class Description Link not found");
                            }
                            catch (NullReferenceException)
                            {
                                classDescriptionuri = td.SelectSingleNode(".//a").GetAttributeValue("href", "Class Description Link not found");
                            }
                            if (!ClassDescriptionsURIs.Contains(classDescriptionuri))
                                ClassDescriptionsURIs.Add(classDescriptionuri);
                        }
                        else
                        { className = "No Class"; classDescriptionuri = "No Description URI"; }
                        ClassesList.Add(new Class(time, currDay, className, classDescriptionuri, classDescription));
                        currDayInt += 1;
                    }
                    //Store unique times
                    if (!Times.Contains(time))
                        Times.Add(time);
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        //Read the data
        private string ReadData(string URL)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string html = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                return html;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
    }
}
