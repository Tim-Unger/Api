using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Api.Controllers.Vatsim.Stats
{
    public class VatsimStats
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("pilotrating")]
        public int PilotRating { get; set; }

        [JsonPropertyName("militaryrating")]
        public int MilitaryRating { get; set; }

        [JsonPropertyName("susp_date")]
        public object SuspensionDate { get; set; }

        [JsonPropertyName("reg_date")]
        public DateTime RegistrationDate { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("division")]
        public string Division { get; set; }
        
        [JsonPropertyName("subdivision")]
        public string SubDivision { get; set; }

        [JsonPropertyName("lastratingchange")]
        public DateTime LastRatingChange { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class StatsController : Controller
    {
        [HttpGet("/vatsim/stats/{cid}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get(string cid = null)
        {
            bool isCid = long.TryParse(cid, out long cidParsed);

            if (!isCid || !IsLengthCorrect(cidParsed))
            {
                return Json("Input was not a valid CID");
            }

            var client = new HttpClient();

            var stats = await client.GetFromJsonAsync<VatsimStats>($"https://api.vatsim.net/api/ratings/{cidParsed}/");

            return Json(stats);
        }

        private static bool IsLengthCorrect(long input) => input.ToString().Length >= 6 && input.ToString().Length <= 7;
    }
}
