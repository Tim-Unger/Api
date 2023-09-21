using static Api.Controllers.AirlinesController;

namespace Api.Controllers.Airlines
{
    internal class Searches
    {
        internal static JsonResult Get(string search)
        {
            var matches = _searchRegex.Matches(search);

            if (matches.Count == 0 || !matches[0].Success)
            {
                return new JsonResult(new ApiError(
                    "Please use one of the following search Parameters: name=, iata=, icao=, callsign=, country="
                ), Options.JsonOptions);
            }

            var airlines = AirlinesJson.ReadJson();

            var searches = GetSearchResults(matches);

            return new JsonResult(SearchResults.Get(searches, airlines, Parameters.SingleSearch), Options.JsonOptions);
        }

        internal static List<Search> GetSearchResults(MatchCollection matches) =>
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
                                //"active" => SearchParameter.Active,
                                _
                                  => throw new ArgumentOutOfRangeException(
                                      "search parameter not recognized"
                                  )
                            },
                            SearchString = x.Groups[2].Value
                        }
                )
                .ToList();
    }
}
