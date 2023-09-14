namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByIata
    {
        internal static AirlineResult Get(
            string searchParameter,
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
                Parameter = searchParameter,
                Airlines = airlines.Where(x => x.Iata.Equals(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).ToList()
            };
        }
    }
}
