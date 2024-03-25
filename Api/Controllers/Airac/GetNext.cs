using AiracGen;

namespace Api.Controllers.Airac
{
    internal partial class Airacs
    {
        internal static JsonResult GetNext() => new(AiracGenerator.GenerateNext(), Options.JsonOptions);
    }
}
