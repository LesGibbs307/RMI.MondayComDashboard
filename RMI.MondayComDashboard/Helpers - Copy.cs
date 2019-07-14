using RMI.MondayComDashboard.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;

namespace RMI.MondayComDashboard
{
    class Helpers
    {
        //private Queue<MondayTest> mondayQueue = new Queue<MondayTest>();
        public void ThreadTest(MondayTest pulse)
        {
            //Thread.Sleep(6000);
            //while (mondayQueue.Count > 0)
            //{
            //    lock (this)
            //    {
            //        Thread.Sleep(6000);
            //    }
            //}

            //if (mondayQueue.Count == 0)
            //{
                //mondayQueue.Enqueue(pulse);
                VerifyingPulses(pulse);
            //}
        }

        //private Task<Queue<MondayTest>> VerifyingPulses()
        private void VerifyingPulses(MondayTest pulse) {
            try
            {
                //lock(this)
                //{
                //    foreach (var item in mondayQueue)
                //    {
                        //bool isNull = CheckIfValuesNull(item);
                        pulse = ConvertPulseEnum(pulse);
                        bool isNull = CheckIfValuesNull(pulse);

                        if (!isNull)
                        {
                            DbHelpers dbHelpers = new DbHelpers();
                            string testType = pulse.testType.ToString();
                            string status = pulse.status.ToString();
                            if (testType == "RMI" && status == "Complete" || status == "Testing")
                            {
                                dbHelpers.AddNewTest(pulse);
                            }
                        }
                    //}
                    //EndThread();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool CheckIfValuesNull(MondayTest pulse)
        {
            foreach (PropertyInfo pi in pulse.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(pulse);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private MondayTest ConvertPulseEnum(MondayTest pulse)
        {
            foreach(var item in pulse.columnInfo)
            {
                if (item.Id.Contains("status"))
                {
                    foreach(PropertyInfo pi in pulse.GetType().GetProperties())
                    {
                        string propName = FirstLetterToUpper(pi.Name);
                        if (item.Title.Trim() == propName)
                        {
                            var thisPulseType = typeof(MondayTest).GetProperty(pi.Name);
                            int labelLength = item.Labels.Count;
                            int counter = 0;

                            foreach (var label in item.Labels)
                            {
                                counter++;
                                if(label.Value.Trim() == pi.GetValue(pulse).ToString())
                                {
                                    var test2 = pi.GetValue(pulse);
                                    var enumLength = 4;
                                    int test = 0;
                                    test = test + 2;
                                }
                            }
                        }
                    }
                }
            }
            return pulse;
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null || str == "")
            {
                return null;
            } else
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }                
        }

        //private void EndThread()
        //{
        //    while(mondayQueue.Count > 0)
        //    {
        //        mondayQueue.Dequeue();
        //    }
        //}
    }
}