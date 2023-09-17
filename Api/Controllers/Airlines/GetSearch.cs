using Api.Controllers.Airlines.SearchParameters;
using static Api.Controllers.Airlines.Parameter;
using static Api.Controllers.AirlinesController;

namespace Api.Controllers.Airlines
{
    internal class SearchResults
    {
        internal static List<AirlineResult> Get(List<Search> searches, List<Airline> airlines, bool matchAllParamters)
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

            if (matchAllParamters)
            {
                var stringBuilder = new StringBuilder();

                var concatResults = new List<AirlineResult>(1) { new AirlineResult() };

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
