using AiracGen;

namespace Api.Controllers.Airac
{
    internal class NextAirac
    {
        internal static JsonResult Get() => new(AiracGenerator.GenerateNext(), Options.JsonOptions);
    }
}
