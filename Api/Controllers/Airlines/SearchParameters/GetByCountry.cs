﻿namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByCountry
    {
        internal static AirlineResult Get(
            string searchParameter,
            string search,
            List<Airline> airlines
        )
        {
            var countries = File.ReadAllLines($"{Environment.CurrentDirectory}/countries.txt")
                .ToList();

            if (!countries.Any(x => x.ToLower() == search.ToLower()))
            {
                throw new InvalidDataException(
                    "Please provide a valid country. see api.tim-u.me/countries for a list of all countries"
                );
            }

            return new AirlineResult()
            {
                Parameter = searchParameter,
                Airlines = airlines
                    .Where(
                        x => x.Country.Equals(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
        }
    }
}
