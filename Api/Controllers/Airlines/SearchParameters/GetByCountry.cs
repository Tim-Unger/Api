namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByCountry
    {
        internal static AirlineResult Get(
            SearchParameter searchParameter,
            string search,
            List<Airline> airlines
        )
        {
            var countries = File.ReadAllLines($"{Environment.CurrentDirectory}/Data/countries.txt")
                .ToList();

            if (!countries.Any(x => x.ToLower() == search.ToLower()))
            {
                throw new InvalidDataException(
                    "Please provide a valid country. see api.tim-u.me/countries for a list of all countries"
                );
            }

            return new AirlineResult()
            {
                Parameters = searchParameter.SingleItemToList(),
                Airlines = airlines
                    .Where(
                        x => x.Country.Equals(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
        }
    }
}
