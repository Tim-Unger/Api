using AviationSharp.NAT;

namespace Api.Controllers
{
    public class NattrakController : Controller
    {
        /// <summary>
        /// Get all Nattracks from the current Nattrack-NOTAM
        /// </summary>
        /// <returns></returns>
        [HttpGet("/nattrack/all")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAllTracks() => Json(NatTracks.GetAll(), Options.JsonOptions);

        /// <summary>
        /// Get a single track by its Indicator from the current Nattrack-NOTAM
        /// </summary>
        /// <param name="indicator"></param>
        /// <returns></returns>
        [HttpGet("/nattrack/{indicator}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetSingleTrack(string indicator) => Json(NatTracks.GetByID(indicator), Options.JsonOptions);

        /// <summary>
        /// Get the raw text from the current Nattrack-NOTAM
        /// </summary>
        /// <returns></returns>
        [HttpGet("/nattrack/notam")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetNatNotam() => NatTracks.GetRawNatNotam();

        /// <summary>
        /// Get todays TMI (number of the current day in the year (based on UTC-Time))
        /// </summary>
        /// <returns></returns>
        [HttpGet("/nattrack/tmi")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public int GetTmi() => NatTracks.GetTodaysTMI;

        /// <summary>
        /// Get all historical Concorde Nattracks
        /// </summary>
        /// <returns></returns>
        [HttpGet("/nattrack/concorde")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetConcordeTracks() => Json(NatTracks.GetConcordeTracks(), Options.JsonOptions);

        /// <summary>
        /// Get a single Concorde-Track by its Indicator from the current Nattrack-NOTAM
        /// </summary>
        /// <param name="indicator"></param>
        /// <returns></returns>
        [HttpGet("/nattrack/concorde/{indicator}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetConcordeTracks(string indicator) => Json(NatTracks.GetConcordeTrack(indicator), Options.JsonOptions);
    }
}
