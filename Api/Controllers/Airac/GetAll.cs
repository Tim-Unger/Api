using AiracGen;

namespace Api.Controllers.Airac
{
    internal partial class Airacs
    {
        //Gets all Airacs
        internal static JsonResult GetAll() => new(GetAiracList(), Options.JsonOptions);

        //Only gets the Airacs from the current, previous and next 3 years
        internal static JsonResult Get()
        {
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var relevantYears = new[] { now.AddYears(-1).Year, now.Year, now.AddYears(1).Year, now.AddYears(2).Year, now.AddYears(3).Year };

            var airacs = GetAiracList();

            var relevantAiracs = airacs.Where(x => relevantYears.Any(y => x.StartDate.Year == y));

            return new(relevantAiracs, Options.JsonOptions);
        }

        internal static List<AiracGen.Airac> GetAiracList() => AiracGenerator.GeneratePastAndFuture(200, 400);
    }
}
