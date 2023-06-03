namespace Api.Controllers.Airlines
{
    internal class AirlinesJson
    {
        internal static List<Airline> ReadJson()
        {
            var airlinesPath = $"{Environment.CurrentDirectory}/airlines.json";
            var airlinesJson = JsonSerializer.Deserialize<List<AirlineDTO>>(
                File.ReadAllText(airlinesPath)
            )!;

            return airlinesJson
                .Select(
                    x =>
                        new Airline()
                        {
                            Name = x.Name,
                            Iata = x.Iata,
                            Icao = x.Icao,
                            Callsign = x.Callsign,
                            Country = x.Country,
                            IsActive = x.Active == "Y"
                        }
                )
                .ToList();
        }
    }
}
