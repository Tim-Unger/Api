using Api.Controllers.Airlines;
using System.Text.RegularExpressions;

namespace Api.Controllers
{
    [Route("api")]
    public class AirlinesController : Controller
    {
        private enum SearchParameter
        {
            Name,
            Iata,
            Icao,
            Callsign,
            Country,
            Active
        }

        /// <summary>
        /// Get the Data about an Airline-ICAO
        /// </summary>
        /// <remarks>
        /// You can use the following search parameters:
        /// name={Name}
        /// iata={Iata},
        /// icao={Icao},
        /// callsign={Callsign},
        /// country={Country},
        /// active={yes(true)/no(false)}
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines/{search}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string search)
        {
            var match = Regex.Match(search, @"(name|iata|icao|callsign|country|active)=(\w*)");
            
            if (!match.Success)
            {
                return Json(
                    "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country=, active="
                );
            }

            var airlinesPath = $"{Environment.CurrentDirectory}/airlines.json";
            var airlinesJson = JsonSerializer.Deserialize<List<AirlineDTO>>(
                System.IO.File.ReadAllText(airlinesPath)
            )!;

            var airlines = airlinesJson
                .Select(
                    x =>
                        new Airline()
                        {
                            Name = x.Name,
                            Iata = x.Iata,
                            Icao = x.Icao,
                            Callsign = x.Callsign,
                            Country = x.Country,
                            IsActive = x.Active == "Y"
                        }
                )
                .ToList();

            var searchParameter = match.Groups[1].Value.ToLower() switch
            {
                "name" => SearchParameter.Name,
                "iata" => SearchParameter.Iata,
                "icao" => SearchParameter.Icao,
                "callsign" => SearchParameter.Callsign,
                "country" => SearchParameter.Country,
                "active" => SearchParameter.Active,
                _ => throw new ArgumentOutOfRangeException("search parameter not recognized")
            };

            var searchValue = match.Groups[2].Value;
            return GetSearch(searchValue, searchParameter, airlines);
        }

        private JsonResult GetSearch(
            string search,
            SearchParameter searchParameter,
            List<Airline> airlines
        ) =>
            searchParameter switch
            {
                SearchParameter.Name => GetByName(search, airlines),
                SearchParameter.Iata => GetByIata(search, airlines),
                SearchParameter.Icao => GetByIcao(search, airlines),
                SearchParameter.Callsign => GetByCallsign(search, airlines),
                SearchParameter.Country => GetByCountry(search, airlines),
                SearchParameter.Active => GetByActive(search, airlines),
                _ => throw new ArgumentOutOfRangeException()
            };

        private JsonResult GetByName(string search, List<Airline> airlines) =>
            Json(
                airlines.Where(
                    x => x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                )
            );

        private JsonResult GetByIata(string search, List<Airline> airlines)
        {
            if (search.Length != 3)
            {
                return Json("Please provide a valid three-letter IATA-Code");
            }

            return Json(airlines.Where(x => x.Iata == search.ToUpper()).FirstOrDefault());
        }

        private JsonResult GetByIcao(string search, List<Airline> airlines)
        {
            if (search.Length != 3)
            {
                return Json("Please provide a valid three-letter ICAO-Code");
            }

            return Json(airlines.Where(x => x.Icao == search.ToUpper()).FirstOrDefault());
        }

        private JsonResult GetByCallsign(string search, List<Airline> airlines) =>
            Json(
                airlines.Where(
                    x => x.Callsign.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                )
            );

        private JsonResult GetByCountry(string search, List<Airline> airlines)
        {
            var countries = System.IO.File.ReadAllLines($"{Environment.CurrentDirectory}/countries.txt").ToList();

            if (!countries.Any(x => x.ToLower() == search.ToLower()))
            {
                return Json(
                    "Please provide a valid country. see api.tim-u.me/countries for a list of all countries"
                );
            }

            return Json(
                airlines
                    .Where(
                        x => x.Country.Equals(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            );
        }

        private JsonResult GetByActive(string search, List<Airline> airlines)
        {
            var isActive = search.ToLower() switch
            {
                "yes" or "y" or "true" or "ye" => true,
                "no" or "n" or "false" or "nah" => false,
                _ => throw new Exception()
            };

            return Json(airlines.Where(x => x.IsActive == isActive).ToList());
        }
    }
}
