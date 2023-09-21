namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class BetriebsrichtungController : Controller
    {
        /// <summary>
        /// Get the current landing direction and forecast for the next days at EDDF as a JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get() => Json(GetBetriebsrichtung.Get(), Options.JsonOptions);

        /// <summary>
        /// Get the current landing direction at EDDF as an int (RWY-direction only, nothing else)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung/raw")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public int GetRaw() => GetBetriebsrichtung.Get().Richtung;

        /// <summary>
        /// Get the current landing direction and the forecast for the next days decoded
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetDecoded() => BetriebsrichtungDecoder.Decode(GetBetriebsrichtung.Get());

        //[HttpGet("/betriebsrichtung/graphic")]
        //[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        //public string GetGraphic() => BetriebsrichtungDrawer.Draw(GetBetriebsrichtung.Get());
    }
}