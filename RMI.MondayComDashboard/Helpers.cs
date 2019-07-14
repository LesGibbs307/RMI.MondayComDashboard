using Newtonsoft.Json;
using RMI.MondayComDashboard.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RMI.MondayComDashboard {
    class Helpers {
        public void VerifyingPulses(MondayTest pulse) {
            try {
                pulse = ConvertPulseEnum(pulse);
                bool isNull = CheckIfValuesNull(pulse);

                if (!isNull) {
                    DbHelpers dbHelpers = new DbHelpers();
                    string testType = pulse.testType.ToString();
                    string status = pulse.status.ToString();
                    if (testType == "RMI" && status == "Complete" || status == "Testing") {
                        dbHelpers.AddNewTest(pulse);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private bool CheckIfValuesNull(MondayTest pulse) {
            foreach (PropertyInfo pi in pulse.GetType().GetProperties()) {
                Type propType = pi.PropertyType;
                bool isEnum = propType == typeof(Status) ||
                              propType == typeof(TestType);
                if (propType == typeof(string) || isEnum) {
                    var value = pi.GetValue(pulse).ToString();
                    if (string.IsNullOrEmpty(value) || value == "None") {
                        return true;
                    }
                }
            }
            return false;
        }

        private MondayTest ConvertPulseEnum(MondayTest pulse) {
            List<Columns> columnInfo = JsonConvert.DeserializeObject<List<Columns>>(pulse.columnInfo);
            foreach (var item in columnInfo) {
                if (item.Id.Contains("status")) {
                    foreach (PropertyInfo pi in pulse.GetType().GetProperties()) {
                        string piName = pi.Name;
                        string propName = FirstLetterToUpper(piName);
                        string propValue = pi.GetValue(pulse).ToString();

                        if (item.Title.Replace(" ", "") == propName) {
                            foreach (var label in item.Labels) {
                                if (label.Key == propValue) {
                                    propValue = label.Value.Replace(" ", "");
                                    switch (piName) {
                                        case "status":
                                            if (propValue == "") {
                                                Status newStatus = Status.None;
                                                pulse.status = newStatus;
                                            } else {
                                                Status newStatus = (Status)Enum.Parse(typeof(Status), propValue);
                                                pulse.status = newStatus;
                                            }
                                            break;
                                        case "testType":
                                            if (propValue == "") {
                                                TestType newTestType = TestType.None;
                                                pulse.testType = newTestType;
                                            } else {
                                                TestType newTestType = (TestType)Enum.Parse(typeof(TestType), propValue);
                                                pulse.testType = newTestType;
                                            }
                                            break;
                                        case "winner":
                                            if (propValue == "") {
                                                Winner newWinner = Winner.None;
                                                pulse.winner = newWinner;
                                            } else {
                                                Winner newWinner = (Winner)Enum.Parse(typeof(Winner), propValue);
                                                pulse.winner = newWinner;
                                            }
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return pulse;
        }

        private string FirstLetterToUpper(string str) {
            if (str == null || str == "") {
                return null;
            } else {
                return char.ToUpper(str[0]) + str.Substring(1);
            }                
        }
    }
}