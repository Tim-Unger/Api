namespace Api.Controllers.Airlines
{
    //Transfer Object to parse from the JSON
    internal class AirlineDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }

        [JsonPropertyName("callsign")]
        public string Callsign { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("active")]
        public string Active { get; set; }
    }

    //The actual Airline Class
    internal class Airline
    {
        public string Name { get; set; } = "Airline";
        public string Iata { get; set; } = "AAA";
        public string Icao { get; set; } = "AAAA";
        public string Callsign { get; set; } = "Callsign";
        public string Country { get; set; } = "Country";
        public bool IsActive { get; set; } = true;
    }
}
