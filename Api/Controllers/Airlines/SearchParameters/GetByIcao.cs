namespace Api.Controllers.Airlines.SearchParameters
{
    public class ByIcao
    {
        internal static AirlineResult Get(
            SearchParameter searchParameter,
            string search,
            List<Airline> airlines
        )
        {
            if (search.Length != 3)
            {
                throw new InvalidDataException("Please provide a valid three-letter ICAO-Code");
            }

            return new AirlineResult()
            {
                Parameters = searchParameter.SingleItemToList(),
                Airlines = airlines.Where(x => x.Icao.Equals(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).ToList()
            };
        }
    }
}
