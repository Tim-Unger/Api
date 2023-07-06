namespace Api.Controllers.Vatsim.Stats
{
    internal class VatsimStats
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
}
