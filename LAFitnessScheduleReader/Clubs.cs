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

        //Private
        private string URL { get; set; }
        private static string states = "|AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|";

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

                    GetData();
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        private bool isStateAbbreviation (string state)
        {
            return state.Length == 2 && states.IndexOf( state ) > 0;
        }

        //Output functions
        public string GetClassSchedules(string Day = null, string Time = null)
        {
            StringBuilder toReturn = new StringBuilder();
            foreach (Club c in this.ClubsList)
            {
                toReturn.Append(c.ToString()).Append("\r\n");
                toReturn.Append(c.ClubAddress).Append("\r\n");
                toReturn.Append(c.GetClassSchedule(Day, Time)).Append("\r\n");
                toReturn.Append("\r\n");
            }
            return toReturn.ToString();
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
                        State = ClubAddress.Substring(ClubAddress.Length - 9, 2);
                        ClubNumber += 1;
                        ClubsList.Add(new Club(State, Zip, ClubURI, ClubName, ClubAddress, ClubDescription, ClubID));
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
    }
}
