namespace Api.Controllers.Airac
{
    internal class ByIdent
    {
        internal static JsonResult Get(string inputIdent)
        {
            if (!int.TryParse(inputIdent, out _))
            {
                return new JsonResult(new ApiError("Provided Input was not a number"));
            }

            var ident = int.Parse(inputIdent);

            return new JsonResult(Airacs.GetList().FirstOrDefault(x => x.Ident == ident))
                ?? new JsonResult(new ApiError("Ident not found"));
        }
    }
}
