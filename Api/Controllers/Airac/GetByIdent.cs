using AiracGen;

namespace Api.Controllers.Airac
{
    internal class ByIdent
    {
        internal static JsonResult Get(string inputIdent)
        {
            if (!int.TryParse(inputIdent, out var ident))
            {
                return new JsonResult(new ApiError("Provided Input was not a number"), Options.JsonOptions);
            }

            var airac = AiracGenerator.GenerateSingle(inputIdent);
            return new JsonResult(airac is not null ? airac : new ApiError("Ident not found"), Options.JsonOptions);
        }
    }
}