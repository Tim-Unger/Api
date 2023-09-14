using AviationSharp.Vatsim;

namespace Api.Controllers.Vatsim.Stats
{
    [Route("api")]
    [ApiController]
    public class StatsController : Controller
    {
        [HttpGet("/vatsim/stats/{cid}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get(string cid)
        {
            var isCid = int.TryParse(cid, out var cidParsed);

            if (!isCid || !VatsimData.DoesCIDExist(cidParsed))
            {
                return Json("Input was not a valid CID");
            }

            var client = new HttpClient();
            var stats = await client.GetFromJsonAsync<VatsimStats>($"https://api.vatsim.net/api/ratings/{cidParsed}/");

            return Json(stats, Options.JsonOptions);
        }
    }
}