namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByName
    {
        internal static AirlineResult Get(SearchParameter searchParameter, string search, List<Airline> airlines) =>
            new()
            {
                Parameters = searchParameter.SingleItemToList(),
                Airlines = airlines
                    .Where(
                        x => x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
    }
}
