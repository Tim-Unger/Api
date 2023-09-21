using AviationSharp.Vatsim;

namespace Api.Controllers.Vatsim.Stats
{
    [Route("api")]
    [ApiController]
    public class StatsController : Controller
    {
        /// <summary>
        /// Get the Vatsim-Stats for a CID
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [HttpGet("/vatsim/stats/{cid}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get(string cid)
        {
            var isCid = int.TryParse(cid, out var cidParsed);

            if (!isCid || !VatsimData.DoesCIDExist(cidParsed))
            {
                return Json(new ApiError("Input was not a valid CID"));
            }

            var stats = AviationSharp.Vatsim.Stats.VatsimStats.GetFullStats(cidParsed);

            return Json(stats, Options.JsonOptions);
        }
    }
}