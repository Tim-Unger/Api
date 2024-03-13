using AiracGen;

namespace Api.Controllers.Airac
{
    public class PreviousAirac
    {
       internal static JsonResult Get() => new(AiracGenerator.GeneratePrevious(), Options.JsonOptions);
    }
}
