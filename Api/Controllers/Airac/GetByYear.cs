namespace Api.Controllers.Airac
{
    internal class ByYear
    {
        internal static JsonResult Get(string inputYear)
        {
            if (!int.TryParse(inputYear, out _))
            {
                return new JsonResult(new ApiError("Provided input was not a number"), Options.JsonOptions);
            }

            var dateRegex = new Regex(@"^(?>(20[2-3]\d)|([2-3]\d))$");

            var dateMatch = dateRegex.Match(inputYear.ToString());

            if (!dateMatch.Success)
            {
                return new JsonResult(
                    new ApiError("Year was not valid, please provide a valid year"),
                    Options.JsonOptions
                );
            }

            var yearRaw = dateMatch.Groups[1].Success
                ? dateMatch.Groups[1].Value
                : $"20{dateMatch.Groups[2].Value}";

            var isYear = int.TryParse(yearRaw, out var year);

            if (!isYear)
            {
                return new JsonResult(
                    new ApiError("Year was not valid, please provide a valid year"),
                    Options.JsonOptions
                );
            }

            //We can use == here as we do not have localization with numbers
            var airacs = Airacs.GetList().Where(x => x.StartDate.Year == year).ToList();
            return new JsonResult(
                airacs.Count > 0 ? airacs : new ApiError("No Airacs found for this year"),
                Options.JsonOptions
            );
        }
    }
}
