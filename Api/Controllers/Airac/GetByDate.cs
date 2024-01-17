namespace Api.Controllers.Airac
{
    internal class ByDate
    {
        internal static JsonResult Get(string inputDate)
        {
            var airacs = Airacs.GetAiracList();

            var dateRegex = new Regex(
                @"(20[2-3][0-9])(?>_|-|)?(0[1-9]|1[0-2])(?>_|-|)?(0[1-9]|1[0-9]|2[0-9]|3[0-1])"
            );

            if (!dateRegex.IsMatch(inputDate))
            {
                return new JsonResult(
                    new ApiError(
                        "Inputted date was not valid, please use an ISO9601 compliant date (20231231)"
                    ),
                    Options.JsonOptions
                );
            }

            var groups = dateRegex.Match(inputDate).Groups;

            var year = int.Parse(groups[1].Value);

            var month = int.Parse(groups[2].Value);

            var day = int.Parse(groups[3].Value);

            var dateOnly = new DateOnly(year, month, day);

            return new JsonResult(
                    airacs.FirstOrDefault(x => x.StartDate < dateOnly && x.EndDate > dateOnly),
                    Options.JsonOptions
                ) ?? new JsonResult(new ApiError("Date has no Airac"), Options.JsonOptions);
        }
    }
}
