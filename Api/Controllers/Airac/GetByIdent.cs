using AiracGen;

namespace Api.Controllers.Airac
{
    internal class ByIdent
    {
        internal static JsonResult Get(string inputIdent)
        {
            //We can use discard as we only need to know if the input is not an int
            if (!int.TryParse(inputIdent, out _))
            {
                return new JsonResult(new ApiError("Provided Input was not a number"), Options.JsonOptions);
            }

            if(inputIdent.Length != 4)
            {
                return new JsonResult(new ApiError("Provided Input was not 4 letters long"), Options.JsonOptions);
            }

            var airac = AiracGenerator.GenerateSingle(inputIdent);
            return new JsonResult(airac is not null ? airac : new ApiError("Ident not found"), Options.JsonOptions);
        }
    }
}