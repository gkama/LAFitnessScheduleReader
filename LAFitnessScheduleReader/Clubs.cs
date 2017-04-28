using System;
using System.Net;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LAFitnessScheduleReader
{
    class Clubs
    {
        //Public
        public string State { get; set; }
        public int Zip { get; set; }
        public List<Club> ClubsList { get; set; }
        public List<string> ClubsURIs { get; set; }
        public List<string> ClubsNames { get; set; }
        public List<string> ClubsAddresses { get; set; }
        public List<string> ClubsDescriptions { get; set; }
        public List<string> ClubsIDs { get; set; }
        public List<string> ZipCodes { get; set; }
        public List<string> States { get; set; }

        //Private
        private string URL { get; set; }
        private static string states = "|AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|";

        /// <summary>
        /// Find LA Fitness clubs based on State and Zip. Sorted based on the distance from the specified Zip.
        /// </summary>
        /// <param name="State">State of wanted Clubs' list</param>
        /// <param name="Zip">Zip of wanted Clubs' list</param>
        public Clubs(string State, int Zip)
        {
            try
            {
                Regex matchZip = new Regex(@"^\d{5}$", RegexOptions.IgnoreCase);
                if (!isStateAbbreviation(State.ToUpper()))
                    throw new Exception("Incorrect format for State");
                else if (!matchZip.Match(Zip.ToString()).Success)
                    throw new Exception("Incorrect format for Zip");
                else
                {
                    this.State = State;
                    this.Zip = Zip;
                    ClubsList = new List<Club>();
                    ClubsURIs = new List<string>();
                    ClubsNames = new List<string>();
                    ClubsAddresses = new List<string>();
                    ClubsDescriptions = new List<string>();
                    ClubsIDs = new List<string>();
                    ZipCodes = new List<string>();
                    States = new List<string>();

                    GetData();
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        private bool isStateAbbreviation (string State)
        {
            return State.Length == 2 && states.IndexOf(State) > 0;
        }

        //Get data
        private void GetData()
        {
            try
            {
                var doc = new HtmlDocument();
                this.URL = ConfigurationManager.AppSettings["FindClubState"] + this.State +
                    ConfigurationManager.AppSettings["FindClubZip"] + this.Zip;
                doc.LoadHtml(ReadData(URL));

                var allClubs = doc.DocumentNode.SelectNodes("//*[contains(@class,'TextDataColumn')]");

                int ClubNumber = 1;
                foreach (var td in allClubs)
                {
                    string ClubURI = string.Empty;
                    string ClubName = string.Empty;
                    string ClubAddress = string.Empty;
                    string ClubDescription = string.Empty;
                    string ClubID = string.Empty;
                    string Zip = string.Empty;
                    string State = string.Empty;
                    if (td.InnerText.Trim() == "Club")
                    {
                        //Ignore
                    }
                    else
                    {
                        ClubURI = td.SelectSingleNode(".//a").GetAttributeValue("href", "Club Lunk not found");
                        ClubName = td.SelectSingleNode(string.Format(".//span[@id='ctl00_MainContent_repClubInfo_ctl0{0}_lblClubDisplayName']", ClubNumber)).InnerText.Trim();
                        ClubAddress = td.SelectSingleNode(string.Format("//span[@id='ctl00_MainContent_repClubInfo_ctl0{0}_lblAddress']", ClubNumber)).InnerHtml.Replace("<br>", ", ").Replace("&nbsp;&nbsp;", " ").Replace("&nbsp;", " ");
                        ClubDescription = td.InnerText.Trim().Replace("&nbsp;", " ");
                        ClubDescription = Regex.Replace(ClubDescription, @"\s+", " ");
                        ClubID = ClubURI.Split('=')[1];
                        Zip = ClubAddress.Substring(ClubAddress.Length - 5, 5);
                        State = ClubAddress.Substring(ClubAddress.Length - 8, 2);
                        ClubNumber += 1;

                        //Update values
                        ClubsURIs.Add(ClubURI);
                        ClubsNames.Add(ClubName);
                        ClubsAddresses.Add(ClubAddress);
                        ClubsDescriptions.Add(ClubDescription);
                        ClubsIDs.Add(ClubID);
                        if (!ZipCodes.Contains(Zip))
                            ZipCodes.Add(Zip);
                        if (!States.Contains(State))
                            States.Add(State);
                        //Add to the list
                        Classes Classes = new Classes(ClubID);
                        ClubsList.Add(new Club(State, Zip, ClubURI, ClubName, ClubAddress, ClubDescription, ClubID, Classes, Classes.GetClassesList()));
                    }  
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




        #region Output Functions - All the printing ones
        //Output functions
        /// <summary>
        /// Gets the Classes schedule for all Clubs. 
        /// Classes are returned based on the Day and Time specified.
        /// If Day and Time are both null, then the schedule for all days and times of the Clubs is returned. 
        /// </summary>
        /// <param name="Day">Day of Classes schedule - ('Sunday', 'Monday', etc. format)</param>
        /// <param name="Time">Time of Classes schedule - ('hh:mm tt' format)</param>
        /// <returns></returns>
        public string GetClassSchedules(string Day = null, string Time = null)
        {
            try
            {
                StringBuilder toReturn = new StringBuilder();
                foreach (Club c in this.ClubsList)
                {
                    toReturn.Append(c.ToString()).Append("\r\n");
                    toReturn.Append(c.GetClassSchedule(Day, Time)).Append("\r\n");
                    toReturn.Append("\r\n");
                }
                return toReturn.ToString().TrimEnd('\r').TrimEnd('\n');
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        /// <summary>
        /// Gets the Classes schedule for Club found from the spcified Name.
        /// Classes are returned based on the Day and Time specified.
        /// If Day and Time are both null, then the schedule for all days and times of the Club is returned. 
        /// </summary>
        /// <param name="Name">Name of Club</param>
        /// <param name="Day">Day of Classes schedule - ('Sunday', 'Monday', etc. format)</param>
        /// <param name="Time">Time of Classes schedule - ('hh:mm tt' format)</param>
        /// <returns></returns>
        public string GetClassSchedules(string Name, string Day = null, string Time = null)
        {
            try
            {
                Club club = GetClub(Name);
                if (club != null)
                {
                    StringBuilder toReturn = new StringBuilder();

                    //Build the string
                    toReturn.Append(club.ToString()).Append("\r\n");
                    toReturn.Append(club.GetClassSchedule(Day, Time)).Append("\r\n");

                    return toReturn.ToString().TrimEnd('\r').TrimEnd('\n');
                }
                else
                    return "No Schedule Available";
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        /// <summary>
        /// Gets the Classes schedule for Club found from the spcified Zip and State.
        /// If a Club does not match the exact State and Zip, the nearest Club's schedule to the Zip is returned.
        /// Classes are returned based on the Day and Time specified.
        /// If Day and Time are both null, then the schedule for all days and times of the Club is returned.
        /// </summary>
        /// <param name="State">State of Club</param>
        /// <param name="Zip">Zip of Club</param>
        /// <param name="Day">Day of Classes schedule - ('Sunday', 'Monday', etc. format)</param>
        /// <param name="Time">Time of Classes schedule - ('hh:mm tt' format)</param>
        /// <returns></returns>
        public string GetClassSchedules(string State, int Zip, string Day = null, string Time = null)
        {
            try
            {
                Club club = GetClub(State, Zip);
                StringBuilder toReturn = new StringBuilder();

                //Build the string
                toReturn.Append(club.ToString()).Append("\r\n");
                toReturn.Append(club.GetClassSchedule(Day, Time)).Append("\r\n");

                return toReturn.ToString().TrimEnd('\r').TrimEnd('\n');
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        /// <summary>
        /// Gets the exact match for State and Zip. If not found, the closest one is returned (based on distance from Zip).
        /// </summary>
        /// <param name="State">State of wanted Club's ID</param>
        /// <param name="Zip">Zip of wanted Club's ID</param>
        /// <returns></returns>
        public string GetClubID(string State, int Zip)
        {
            foreach (Club c in this.ClubsList)
            {
                if (c.State.ToUpper().Equals(State.Trim().ToUpper())
                    && c.Zip.Equals(Zip.ToString()))
                    return c.ClubID;
            }
            //If none of the exact match, then find the closest one (aka the first one on the list)
            return this.ClubsList[0].ClubID;
        }


        //Private function - GetClub
        private Club GetClub(string State, int Zip)
        {
            foreach (Club c in this.ClubsList)
                if (c.State.ToUpper().Equals(State.Trim().ToUpper())
                    && c.Zip.Equals(Zip.ToString()))
                    return c;
            //If none of the exact match, then find the closest one (aka the first one on the list)
            return this.ClubsList[0];
        }
        //Private function - GetClub
        private Club GetClub(string Name)
        {
            foreach (Club c in this.ClubsList)
                if (c.ClubName.Equals(Name))
                    return c;
            return null;
        }





        /// <summary>
        /// Gets the Clubs' URIs as a string.
        /// </summary>
        /// <returns></returns>
        public string GetClubsURIs()
        {
            return Get(this.ClubsURIs);
        }
        /// <summary>
        /// Gets the Clubs' names as a string.
        /// </summary>
        /// <returns></returns>
        public string GetClubsNames()
        {
            return Get(this.ClubsNames);
        }
        /// <summary>
        /// Gets the Clubs' addresses as a string.
        /// </summary>
        /// <returns></returns>
        public string GetClubsAddresses()
        {
            return Get(this.ClubsAddresses);
        }
        /// <summary>
        /// Gets the Clubs' descriptions as a string.
        /// </summary>
        /// <returns></returns>
        public string GetClubsDescriptions()
        {
            return Get(this.ClubsDescriptions);
        }
        /// <summary>
        /// Gets the Clubs' IDs as a string.
        /// </summary>
        /// <returns></returns>
        public string GetClubsIDs()
        {
            return Get(this.ClubsIDs);
        }
        /// <summary>
        /// Gets the Clubs' zip codes as a string.
        /// </summary>
        /// <returns></returns>
        public string GetZipCodes()
        {
            return Get(this.ZipCodes);
        }
        /// <summary>
        /// Gets the Clubs' states as a string.
        /// </summary>
        /// <returns></returns>
        public string GetStates()
        {
            return Get(this.States);
        }
        private string Get(List<string> List)
        {
            StringBuilder toReturn = new StringBuilder();
            foreach (string s in List)
                toReturn.Append(s).Append("\r\n");
            return toReturn.ToString().TrimEnd('\r').TrimEnd('\n');
        }
        /// <summary>
        /// Get the Clubs' Names and IDs in Dictionary objetc.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetClubsNamesIDs()
        {
            try
            {
                Dictionary<string, string> toReturn = new Dictionary<string,string>();
                foreach (Club c in this.ClubsList)
                    if (!toReturn.ContainsKey(c.ClubName))
                        toReturn.Add(c.ClubName, c.ClubID);
                return toReturn;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        /// <summary>
        /// Returns Clubs' name and addresses
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder toReturn = new StringBuilder();
            foreach (Club c in this.ClubsList)
                toReturn.Append(c.ToString()).Append("\r\n");
            return toReturn.ToString().TrimEnd('\r').TrimEnd('\n');
        }
        #endregion
    }
}
