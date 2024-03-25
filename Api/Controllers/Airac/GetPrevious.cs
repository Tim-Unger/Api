using AiracGen;

namespace Api.Controllers.Airac
{
    internal partial class Airacs
    {
       internal static JsonResult GetPrevious() => new(AiracGenerator.GeneratePrevious(), Options.JsonOptions);
    }
}
