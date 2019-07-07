using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RMI.MondayComDashboard.Models
{
    public partial class LandingPages
    {
        [JsonProperty("pulse")]
        public Pulse Pulse { get; set; }

        [JsonProperty("board_meta")]
        public BoardMeta BoardMeta { get; set; }

        [JsonProperty("column_values")]
        public List<ColumnValue> ColumnValues { get; set; }

        private int GetTime(DateTime? updateTime)
        {
            int timeDifference = 100;
            if(updateTime != null)
            {
                DateTime newUpdateTime = (DateTime)updateTime;
                var currentTime = DateTime.Now;
                TimeSpan difference = currentTime.Subtract(newUpdateTime);
                timeDifference = (int)difference.TotalMinutes;           
            }
            return timeDifference;
        }

        public List<LandingPages> GetTestingResults(List<LandingPages> pulseResult, List<Columns> columns, List<LandingPages> landingPages)
        {
            foreach(var page in landingPages) {
                Columns column = new Columns();
                string boardName = "";
                string testIndex = "";
                string[] test = { testIndex, boardName }; // cleaner way to do this
                column.GetColumnIndex(columns, test);
                foreach (var colVal in page.ColumnValues) {
                    if (colVal.Value != null && colVal.Value.ValueClass != null) {  // cleaner way to do this
                        long? thisColVal = colVal.Value.ValueClass.Index;
                        if(thisColVal != null && colVal.Cid.Contains("status")) {  // cleaner way to do this
                            int convertColVal = Convert.ToInt32(thisColVal);
                            if(convertColVal == Convert.ToInt32(test[0]) && colVal.Title == test[1]) {
                                var updateTime = colVal.Value.ValueClass.ChangedAt;
                                int timeDifference = GetTime(updateTime);// cleaner way to do this
                                if (timeDifference <= 30)
                                {
                                    pulseResult.Add(page);
                                }
                            }
                        }
                    }
                }
            }
            return pulseResult;
        }
    }

    public partial class BoardMeta
    {
        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }
    }

    public partial class ColumnValue
    {
        [JsonProperty("cid", NullValueHandling = NullValueHandling.Ignore)]
        public string Cid { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("value")]
        public ValueUnion Value { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class ValueClass
    {
        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public long? Index { get; set; }

        [JsonProperty("changed_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ChangedAt { get; set; }

        [JsonProperty("update_id")]
        public object UpdateId { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }
    }

    public partial class Pulse
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("updates_count")]
        public long UpdatesCount { get; set; }

        [JsonProperty("board_id")]
        public long BoardId { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public partial class ValueUnion
    {
        [JsonProperty]
        public string String { get; set; }

        [JsonProperty]
        public ValueClass ValueClass { get; set; }

        /*
        public static implicit operator ValueUnion(string String) => new ValueUnion { String = String };
        public static implicit operator ValueUnion(ValueClass ValueClass) => new ValueUnion { ValueClass = ValueClass };        */
        public bool IsNull => ValueClass == null && String == null;


    }

    public partial class LandingPages
    {
        public static List<LandingPages> FromJson(string json) => JsonConvert.DeserializeObject<List<LandingPages>>(json, RMI.MondayComDashboard.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<LandingPages> self) => JsonConvert.SerializeObject(self, RMI.MondayComDashboard.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ValueUnionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ValueUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ValueUnion) || t == typeof(ValueUnion);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new ValueUnion { };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ValueUnion { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ValueClass>(reader);
                    return new ValueUnion { ValueClass = objectValue };
            }
            throw new Exception("Cannot unmarshal type ValueUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ValueUnion)untypedValue;
            if (value.IsNull)
            {
                serializer.Serialize(writer, null);
                return;
            }
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.ValueClass != null)
            {
                serializer.Serialize(writer, value.ValueClass);
                return;
            }
            throw new Exception("Cannot marshal type ValueUnion");
        }

        public static readonly ValueUnionConverter Singleton = new ValueUnionConverter();
    }
}
