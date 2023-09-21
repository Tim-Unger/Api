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

            var airac = Airacs.GetList().FirstOrDefault(x => x.Ident == ident);
            return new JsonResult(airac is not null ? ident : new ApiError("Ident not found"), Options.JsonOptions);

        }
    }
}
