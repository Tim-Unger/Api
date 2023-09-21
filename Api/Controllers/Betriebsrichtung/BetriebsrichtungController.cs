namespace Api.Controllers
{
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
        public JsonResult Get() => Json(GetBetriebsrichtung.Get());

        [HttpGet("/betriebsrichtung/raw")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetRaw() => GetBetriebsrichtung.Get().Richtung.ToString();

        [HttpGet("/betriebsrichtung/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetDecoded() => BetriebsrichtungDecoder.Decode(GetBetriebsrichtung.Get());

        //[HttpGet("/betriebsrichtung/graphic")]
        //[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        //public string GetGraphic() => BetriebsrichtungDrawer.Draw(GetBetriebsrichtung.Get());
    }
}