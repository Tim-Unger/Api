namespace Api.Controllers
{
    internal class BetriebsrichtungDTO
    {
        public string Betriebsrichtung { get; set; } = "25";
    }

    [Route("api")]
    [ApiController]
    public class BetriebsrichtungController : Controller
    {
        //TODO weird behavior
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get()
        {
            var is25 = GetBetriebsrichtung.Betriebsrichtung() == "25";

            var betriebsRichtung = new BetriebsrichtungDTO { Betriebsrichtung = is25 ? "25" : "07" };
            
            return Json(betriebsRichtung);
        }

        [HttpGet("/betriebsrichtung/raw")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetRaw() => GetBetriebsrichtung.Betriebsrichtung() == "25" ? "25" : "07";
    }
}