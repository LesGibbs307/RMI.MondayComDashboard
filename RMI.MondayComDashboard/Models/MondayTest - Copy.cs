using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace RMI.MondayComDashboard.Models
{
   enum Winner {
        Champ,
        Challenger,
        Iterative,
        Malfunction
    }

    public enum TestType
    {
        Optimize,
        RMI        
    }

    //public enum BoardName
    //{
    //    Done,
    //    PreTest,
    //    Testing
    //}

    public enum Status
    {
        Champ,
        Complete,
        Designed,
        Developed,
        Paused,
        PreTest,
        QA,
        Testing
    }

    public class MondayTest
    {
        [JsonProperty("pulseUrl")]
        public string pulseUrl { get; set; }

        [JsonProperty("pulseId")]
        public long pulseId { get; set; }

        [JsonProperty("testName")]
        public string testName { get; set; }

        [JsonProperty("testType")]
        public TestType testType { get; set; }

        [JsonProperty("srcValue")]
        public string srcValue { get; set; }

        [JsonProperty("champUrl")]
        public string champUrl { get; set; }

        [JsonProperty("challengerUrl")]
        public string challengerUrl { get; set; }

        [JsonProperty("status")]
        public Status status { get; set; }

        [JsonProperty("winner")]
        Winner winner { get; set; }

        [JsonProperty("columnInfo")]
        public List<Columns> columnInfo { get; set; }

        public string GetPgValue(string url)
        {
            Uri uri = new Uri(url);
            NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);
            string pg = query["pg"];
            return pg;
        }

        public string CleanUrl(string url)
        {
            Uri uri = new Uri(url);
            url = uri.Authority;
            return url;
        }

        public string DetermineWinner(string champPg, string challengerPg)
        {
            if(this.winner.ToString() == "Challenger")
            {
                return challengerPg;
            }
            else
            {
                return champPg;
            }
        }

        public string GetPath(string url)
        {//defensive programming for invalid urls
            Uri uri = new Uri(url);
            string path = uri.AbsolutePath;
            return path;
        }
    }
}