namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByCallsign
    {
        internal static AirlineResult Get(
            SearchParameter searchParameter,
            string search,
            List<Airline> airlines
        ) =>
            new()
            {
                Parameters = searchParameter.SingleItemToList(),
                Airlines = airlines
                    .Where(
                        x =>
                            x.Callsign.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
    }
}
