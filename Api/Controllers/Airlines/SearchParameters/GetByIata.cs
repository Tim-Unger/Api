namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByIata
    {
        internal static AirlineResult Get(
            SearchParameter searchParameter,
            string search,
            List<Airline> airlines
        )
        {
            if (search.Length != 2)
            {
                throw new InvalidDataException("Please provide a valid two-letter IATA-Code");
            }

            return new AirlineResult()
            {
                Parameters = searchParameter.SingleItemToList(),
                Airlines = airlines.Where(x => x.Iata.Equals(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).ToList()
            };
        }
    }
}
