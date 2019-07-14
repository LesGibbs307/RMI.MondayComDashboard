using RMI.MondayComDashboard.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RMI.MondayComDashboard {
    public class DbHelpers {
        public void AddNewTest(MondayTest pulse) {          
            // get new uri obj here and set values here
            string cs = ConfigurationManager.ConnectionStrings["ABTestingConnectionString"].ConnectionString; // encapuslate this or readonly property 
            string champPg = pulse.GetPgValue(pulse.champUrl);
            string challengerPg = pulse.GetPgValue(pulse.challengerUrl);
            bool isTestEnded = pulse.status.ToString() == "Complete";
            string pulseUrl = pulse.CleanUrl(pulse.champUrl);
            string path = pulse.GetPath(pulse.champUrl);
            string thisWinner = pulse.DetermineWinner(champPg, challengerPg);

            using (SqlConnection conn = new SqlConnection(cs)) {
                try {
                    using (SqlCommand comm = conn.CreateCommand()) {
                        comm.CommandText = "dbo.sp_SetABTestingInfo";
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.AddWithValue("@TestName", pulse.testName); 
                        comm.Parameters.AddWithValue("@Src", pulse.srcValue);
                        comm.Parameters.AddWithValue("@ControlPg", champPg);
                        comm.Parameters.AddWithValue("@ChallengerPg", challengerPg);
                        comm.Parameters.AddWithValue("@Domain", pulseUrl);
                        comm.Parameters.AddWithValue("@Path", path);
                        comm.Parameters.AddWithValue("@IsTestEnding", SqlDbType.Bit).Value = isTestEnded;
                        comm.Parameters.AddWithValue("@Winner", thisWinner);
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}