using Api.Controllers.Airlines;
using Api.Controllers.Airlines.SearchParameters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using static Api.Controllers.Airlines.Parameter;

namespace Api.Controllers
{
    [Route("api")]
    public class AirlinesController : Controller
    {
        private static readonly Regex _searchRegex = new Regex(
            @"(?>(name|iata|icao|callsign|country|active)=(\w*))*(?:(?>&|$))"
        );

        private static List<Airline> Airlines = new List<Airline>();
        private class Search
        {
            public string SearchString { get; set; }
            public SearchParameter SearchParameter { get; set; }
        }

        /// <summary>
        /// Get All Airlines
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airlines")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAll() => Json(AirlinesJson.ReadJson());

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
        /// You can combine searches with an &
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines/{search}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string search)
        {
            var matches = _searchRegex.Matches(search);

            if (matches.Count == 0 || !matches[0].Success)
            {
                return Json(
                    "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country=, active="
                );
            }

            Airlines = AirlinesJson.ReadJson();

            var searches = GetSearches(matches);

            return Json(GetSearch(searches, Airlines, false));
        }

        [HttpGet("/airlines/{search}/matchall")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetMultiple(string search) 
        {
            var matches = _searchRegex.Matches(search);

            if (matches.Count == 0 || !matches[0].Success)
            {
                return Json(
                    "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country=, active="
                );
            }

            if(matches.Count < 2)
            {
                return Json("Please specify at lease 2 search paramters");
            }

            Airlines = AirlinesJson.ReadJson();

            var searches = GetSearches(matches);

            return Json(GetSearch(searches, Airlines, true));
        }

        private List<Search> GetSearches(MatchCollection matches) =>
            matches
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(
                    x =>
                        new Search()
                        {
                            SearchParameter = x.Groups[1].Value.ToLower() switch
                            {
                                "name" => SearchParameter.Name,
                                "iata" => SearchParameter.Iata,
                                "icao" => SearchParameter.Icao,
                                "callsign" => SearchParameter.Callsign,
                                "country" => SearchParameter.Country,
                                "active" => SearchParameter.Active,
                                _
                                  => throw new ArgumentOutOfRangeException(
                                      "search parameter not recognized"
                                  )
                            },
                            SearchString = x.Groups[2].Value
                        }
                )
                .ToList();

        private List<AirlineResult> GetSearch(List<Search> searches, List<Airline> airlines, bool matchAllParamters)
        {
            var results = new List<AirlineResult>();

            searches.ForEach(
                x =>
                    results.Add(
                        x.SearchParameter switch
                        {
                            SearchParameter.Name
                              => ByName.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                            SearchParameter.Iata
                              => ByIata.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                            SearchParameter.Icao
                              => ByIcao.Get(x.SearchParameter.ToString(), x.SearchString, airlines),
                            SearchParameter.Callsign
                              => ByCallsign.Get(
                                  x.SearchParameter.ToString(),
                                  x.SearchString,
                                  airlines
                              ),
                            SearchParameter.Country
                              => ByCountry.Get(
                                  x.SearchParameter.ToString(),
                                  x.SearchString,
                                  airlines
                              ),
                            SearchParameter.Active
                              => ByActive.Get(
                                  x.SearchParameter.ToString(),
                                  x.SearchString,
                                  airlines
                              ),
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    )
            );

            if(matchAllParamters)
            {
                var stringBuilder = new StringBuilder();

                var concatResults = new List<AirlineResult>(1) { new AirlineResult()};

                var last = searches.Last().SearchParameter.ToString();
                searches.Remove(searches.Last());

                searches.ForEach(x => stringBuilder.Append($"{x.SearchParameter.ToString()}, "));
                stringBuilder.Append(last);

                concatResults.First().Parameter = stringBuilder.ToString();

                concatResults.First().Airlines = 
                results
                .SelectMany(x => x.Airlines) //Squash all the Sublists of all Results into one List
                .ToList()
                .GroupBy(x => x.Name) //Sorts them by Name
                .Where(x => x.Count() == results.Count)//Only Airlines that appear multiple times need to be considered (these match all Parameters)
                .Select(x => x.First())
                .ToList(); 

                return concatResults;
            }

            return results;
        }
    }
}
