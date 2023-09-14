using AviationSharp.NAT;

namespace Api.Controllers
{
    public class NattrakController : Controller
    {
        [HttpGet("/nattrack/all")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAllTracks() => Json(NatTracks.GetAll());

        [HttpGet("/nattrack/{indicator}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetSingleTrack(string indicator) => Json(NatTracks.GetByID(indicator));

        [HttpGet("/nattrack/notam")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetNatNotam() => NatTracks.GetRawNatNotam();

        [HttpGet("/nattrack/tmi")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public int GetTmi() => NatTracks.GetTodaysTMI;

        [HttpGet("/nattrack/concorde")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetConcordeTracks() => Json(NatTracks.GetConcordeTracks());

        [HttpGet("/nattrack/concorde/{indicator}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetConcordeTracks(string indicator) => Json(NatTracks.GetConcordeTrack(indicator));
    }
}
