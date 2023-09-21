namespace Api.Controllers.Airlines
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum SearchParameter
    {
        Name,
        Iata,
        Icao,
        Callsign,
        Country,
        Active
    }
}
