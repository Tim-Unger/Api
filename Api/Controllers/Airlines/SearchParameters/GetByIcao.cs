namespace Api.Controllers.Airlines.SearchParameters
{
    public class ByIcao
    {
        internal static AirlineResult Get(
            string searchParameter,
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
                Parameter = searchParameter,
                Airlines = airlines.Where(x => x.Icao.Equals(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).ToList()
            };
        }
    }
}
