namespace Api.Controllers
{
    public enum Richtung
    {
        TwoFive = 1,
        ZeroSeven,
        ChangingZeroSevenToTwoFive = 21,
        ChangingBetweenBoth = 13,
        TwoFiveMaybeChanging = 32,
    }

    internal class Betriebsrichtung
    {
        [JsonPropertyName("betriebsrichtung")]
        public string Richtung { get; set; } = "25";

        public List<Probability> Probabilites { get; set; } = new List<Probability>();
    }

    internal class Probability
    {
        public DateTime ProbabilityStart { get; set; }

        [JsonPropertyName("startDate")]
        public DateOnly ProbabilityStartDate { get;set; }

        [JsonPropertyName("startTime")]
        public TimeOnly ProbabiltyStartTime { get;set; }

        [JsonPropertyName("betriebsrichtung")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Richtung Richtung { get;set; }

        public int ProbabilityPercent { get;set; }
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
        public JsonResult Get() => Json(GetBetriebsrichtung.Get());

        [HttpGet("/betriebsrichtung/raw")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetRaw() => GetBetriebsrichtung.Get().Richtung.ToString();
    }
}