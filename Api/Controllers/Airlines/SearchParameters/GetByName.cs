namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByName
    {
        internal static AirlineResult Get(string searchParameter, string search, List<Airline> airlines) =>
            new()
            {
                Parameter = searchParameter,
                Airlines = airlines
                    .Where(
                        x => x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
    }
}
