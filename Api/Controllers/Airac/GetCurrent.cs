namespace Api.Controllers.Airac
{
    internal class Current
    {
        internal static JsonResult Get()
        {
            var airacs = Airacs.GetList();

            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

            for (var i = 0; i < airacs.Count; i++)
            {
                if (airacs[i + 1].StartDate > dateNow)
                {
                    return new JsonResult(airacs[i]);
                }
            }

            //If you are here then you are somehow later in time than the publication of any airac (or you have time travelled)
            return new JsonResult(new ApiError("You are either a time traveller or there are no published AIRAC Cycles"));
        }
    }
}
