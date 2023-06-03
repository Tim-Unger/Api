using Api.Controllers.Airlines;
using Api.Controllers.Airlines.SearchParameters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using static Api.Controllers.Airlines.Parameter;

namespace Api.Controllers
{
    [Route("api")]
    public class AirlinesController : Controller
    {
        private class Search
        {
            public string SearchString { get; set; }
            public SearchParameter SearchParameter { get; set; }
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
            var matches = Regex.Matches(search, @"(?>(name|iata|icao|callsign|country|active)=(\w*))*(?:(?>&|$))");

            if (matches.Count == 0 || !matches[0].Success)
            {
                return Json(
                    "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country=, active="
                );
            }

            var airlines = AirlinesJson.ReadJson();

            var searches = matches
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select
                    (x => new Search()
                    {
                        SearchParameter = x.Groups[1].Value.ToLower() switch
                        {
                            "name" => SearchParameter.Name,
                            "iata" => SearchParameter.Iata,
                            "icao" => SearchParameter.Icao,
                            "callsign" => SearchParameter.Callsign,
                            "country" => SearchParameter.Country,
                            "active" => SearchParameter.Active,
                            _ => throw new ArgumentOutOfRangeException("search parameter not recognized")
                        },
                        SearchString = x.Groups[2].Value
                    })
            .ToList();

            return Json(GetSearch(searches, airlines));
        }

        /// <summary>
        /// Get All Airlines
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airlines")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAll() => Json(AirlinesJson.ReadJson());

        private List<AirlineResult> GetSearch(List<Search> searches, List<Airline> airlines)
        {
            var results = new List<AirlineResult>();

            searches.ForEach(x =>
                results.Add(
                    x.SearchParameter switch
                    {
                        SearchParameter.Name => ByName.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        SearchParameter.Iata => ByIata.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        SearchParameter.Icao => ByIcao.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        SearchParameter.Callsign => ByCallsign.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        SearchParameter.Country => ByCountry.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        SearchParameter.Active => ByActive.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                        _ => throw new ArgumentOutOfRangeException()
                    })
            );

            var json = JsonSerializer.Serialize(results);
            return results;
        }
    }
}
