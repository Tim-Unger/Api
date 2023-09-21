using static Api.Controllers.AirlinesController;

namespace Api.Controllers.Airlines
{
    internal class GetSearchesMultipleParameters
    {
        internal static JsonResult Get(string search, bool matchAll)
        {
            var matches = _searchRegex.Matches(search);

            if (matches.Count == 0 || !matches[0].Success)
            {
                return new JsonResult(
                    new ApiError(
                        "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country=, active="
                    )
                );
            }

            if (matches.Count < 2)
            {
                return new JsonResult(new ApiError("Please specify at lease 2 search paramters"));
            }

            var airlines = AirlinesJson.ReadJson();

            var searches = Searches.GetSearchResults(matches);

            return matchAll
              ? new JsonResult(SearchResults.Get(searches, airlines, Parameters.MatchAll))
              : new JsonResult(SearchResults.Get(searches, airlines, Parameters.MatchAny));
        }
    }
}
