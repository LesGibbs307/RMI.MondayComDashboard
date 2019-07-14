using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace RMI.MondayComDashboard.Models {
   public enum Winner {
        None = 50, 
        Control = 100,
        Challenger = 101,
        Iterative = 102,
        Malfunction = 103
    }

    public enum TestType {
        None = 50,
        Optimize = 100,
        RMI = 101
    }

    public enum Status {
        None = 50,
        Champ = 100,
        Complete = 101,
        Paused = 102,
        Designed = 103,
        Developed = 104,
        PreTest = 105,
        QA = 106,
        Testing = 107
    }

    public class MondayTest {
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
        public Winner winner { get; set; }

        [JsonProperty("columnInfo")]
        public string columnInfo { get; set; }

        public string GetPgValue(string url) {
            Uri uri = new Uri(url);
            NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);
            string pg = query["pg"];
            return pg;
        }

        public string CleanUrl(string url) {
            Uri uri = new Uri(url);
            url = uri.Authority;
            return url;
        }

        public string DetermineWinner(string champPg, string challengerPg) {
            string winnerResult = this.winner.ToString();
            if(winnerResult == "Control" || winnerResult == "None") {
                return champPg;
            } else {
                return challengerPg;
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
