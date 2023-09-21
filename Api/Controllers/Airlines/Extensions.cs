namespace Api.Controllers.Airlines
{
    internal static class Extensions
    {
        internal static List<SearchParameter> SingleItemToList(this SearchParameter searchParameter) => new(1) { searchParameter };

        internal static List<AirlineResult> SingleItemToList(this AirlineResult airlineResult) => new(1) { airlineResult };
    }
}
