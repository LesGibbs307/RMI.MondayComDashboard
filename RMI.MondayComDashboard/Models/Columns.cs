using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RMI.MondayComDashboard.Models
{
    public partial class Columns
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public double? Width { get; set; }

        [JsonProperty("labels", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Labels { get; set; }

        [JsonProperty("labels_positions_v2", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, long> LabelsPositionsV2 { get; set; }

        public string[] GetColumnIndex(List<Columns> columns, string[] test)
        {
            foreach (var column in columns) {
                if (column.Labels != null) {
                    foreach(var label in column.Labels) {
                        if (label.Value.Trim() == "Testing") {
                            test[0] = label.Key; // cleaner way to do this
                            test[1] = column.Title; // cleaner way to do this
                            //index = Int32.Parse(label.Key);
                            //boardName = "test";
                        }
                    }
                }
            }
            return test;
        }
    }
}
