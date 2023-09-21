namespace Api.Controllers.Airlines
{
    //The actual Airline Class
    internal class Airline
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Airline";

        [JsonPropertyName("iata")]
        public string Iata { get; set; } = "AAA";

        [JsonPropertyName("icao")]
        public string Icao { get; set; } = "AAAA";

        [JsonPropertyName("callsign")]
        public string Callsign { get; set; } = "Callsign";

        [JsonPropertyName("country")]
        public string Country { get; set; } = "Country";

        //[JsonPropertyName("active")]
        //internal string _isActive;

        //[JsonPropertyName("isActive")]
        //public bool IsActive = true;
    }
}
