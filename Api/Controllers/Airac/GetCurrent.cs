using AiracGen;

namespace Api.Controllers.Airac
{
    internal partial class Airacs
    {
        internal static JsonResult GetCurrent() => new(AiracGenerator.GenerateCurrent()); //Exceptions are handled in the function
    }
}
