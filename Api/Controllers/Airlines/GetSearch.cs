using Api.Controllers.Airlines.SearchParameters;
using static Api.Controllers.AirlinesController;

namespace Api.Controllers.Airlines
{
    internal enum Parameters
    {
        SingleSearch,
        MatchAll,
        MatchAny
    }

    internal class SearchResults
    {
        internal static List<AirlineResult> Get(
            List<Search> searches,
            List<Airline> airlines,
            Parameters parameters
        )
        {
            var results = new List<AirlineResult>();

            searches.ForEach(
                x =>
                    results.Add(
                        x.SearchParameter switch
                        {
                            SearchParameter.Name
                              => ByName.Get(x.SearchParameter, x.SearchString, airlines),
                            SearchParameter.Iata
                              => ByIata.Get(x.SearchParameter, x.SearchString, airlines),
                            SearchParameter.Icao
                              => ByIcao.Get(x.SearchParameter, x.SearchString, airlines),
                            SearchParameter.Callsign
                              => ByCallsign.Get(x.SearchParameter, x.SearchString, airlines),
                            SearchParameter.Country
                              => ByCountry.Get(x.SearchParameter, x.SearchString, airlines),
                            //SearchParameter.Active
                            //    => ByActive.Get(
                            //        x.SearchParameter.ToString(),
                            //        x.SearchString,
                            //        airlines
                            //    ),
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    )
            );

            if (parameters == Parameters.SingleSearch)
            {
                return results;
            }

            if (parameters == Parameters.MatchAll)
            {
                var matchAllResults = new AirlineResult
                {
                    Parameters = searches.Select(x => x.SearchParameter),
                    Airlines = results
                        .SelectMany(x => x.Airlines) //Squash all the Sublists of all Results into one List
                        .GroupBy(x => x.Name) //Sorts them by Name
                        .Where(x => x.Count() == results.Count) //Only Airlines that appear as many times as search parameters match all values and need to be considered
                        .Select(x => x.First()) //Gets the first items from the group results (doesn't matter which one we take from the results since they are all the same airline anyways)
                        .ToList()
                };

                return matchAllResults.SingleItemToList();
            }

            var matchAnyResults = new AirlineResult()
            {
                Parameters = searches.Select(x => x.SearchParameter),
                Airlines = results
                    .SelectMany(x => x.Airlines)
                    .GroupBy(x => x.Name)
                    .Select(x => x.First())
                    .ToList()
            };

            return matchAnyResults.SingleItemToList();
        }
    }
}
