using static Api.Controllers.AirlinesController;

namespace Api.Controllers.Airlines.SearchParameters
{
    internal class ByCallsign
    {
        internal static AirlineResult Get(
            string searchParameter,
            string search,
            List<Airline> airlines
        ) =>
            new AirlineResult()
            {
                Parameter = searchParameter,
                Airlines = airlines
                    .Where(
                        x =>
                            x.Callsign.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .ToList()
            };
    }
}
