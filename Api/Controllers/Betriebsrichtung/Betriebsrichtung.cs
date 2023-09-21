namespace Api.Controllers
{
    internal class Betriebsrichtung
    {
        [JsonPropertyName("betriebsrichtung")]
        public int Richtung { get; set; } = 25;

        public List<Probability> Probabilites { get; set; } = new List<Probability>();
    }

    internal class Probability
    {
        public DateTime ProbabilityStart { get; set; }

        [JsonPropertyName("startDate")]
        public DateOnly ProbabilityStartDate { get; set; }

        [JsonPropertyName("startTime")]
        public TimeOnly ProbabiltyStartTime { get; set; }

        [JsonPropertyName("betriebsrichtung")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Richtung Richtung { get; set; }

        public int ProbabilityPercent { get; set; }
    }

    public enum Richtung
    {
        TwoFive,
        ZeroSeven,
        BothPossible,
        ZeroSevenMaybeChanging,
        ChangingTwoFiveToZeroSeven,
        ChangingZeroSevenToTwoFive,
        ChangingBetweenBoth,
        TwoFiveMaybeChanging,
    }
}
