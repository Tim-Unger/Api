using AiracGen;

namespace Api.Controllers.Airac
{
    internal class CurrentAirac
    {
        internal static JsonResult Get() => new JsonResult(AiracGenerator.GenerateCurrent()); //Exceptions are handled in the function
    }
}
