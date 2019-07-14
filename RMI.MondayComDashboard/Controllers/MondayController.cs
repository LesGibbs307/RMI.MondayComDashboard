using RMI.MondayComDashboard.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace RMI.MondayComDashboard.Controllers
{
    public class MondayController : ApiController
    {
        static string pulseUrl = "https://api.monday.com/v1/boards/213532600/pulses.json?api_key=11f160f3214b9584da1c06f27871bee3";
        static string columnUrl = "https://api.monday.com/v1/boards/213532600/columns.json?api_key=11f160f3214b9584da1c06f27871bee3";
        static Helpers threadHelp = new Helpers();

        // POST: api/Monday
        public void Post(MondayTest pulse) {
            try {
                threadHelp.VerifyingPulses(pulse);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<List<LandingPages>> Get() {
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (HttpResponseMessage boardResponse = await client.GetAsync(pulseUrl)) {
                    using (HttpResponseMessage columnResponse = await client.GetAsync(columnUrl)) {
                        if (boardResponse.IsSuccessStatusCode && columnResponse.IsSuccessStatusCode) {
                            var jsonResult = await boardResponse.Content.ReadAsStringAsync();
                            var landingPages = LandingPages.FromJson(jsonResult);
                            var columnResult = await columnResponse.Content.ReadAsStringAsync();
                            List<Columns> columns = JsonConvert.DeserializeObject<List<Columns>>(columnResult);
                            LandingPages testPage = new LandingPages();
                            List<LandingPages> pulseResult = new List<LandingPages>();
                            testPage.GetTestingResults(pulseResult, columns, landingPages);
                            return pulseResult;
                        } else if (!boardResponse.IsSuccessStatusCode) {
                            throw new Exception(boardResponse.ReasonPhrase);
                        } else {
                            throw new Exception(columnResponse.ReasonPhrase);
                        }
                    }
                }
            }
        }
    }
}