using static Api.Controllers.DAtis.DAtisAirportsClass;

namespace Api.Controllers.DAtis
{
    internal class Datis
    {
        internal static JsonResult Get(string icao, string? isTextOnly)
        {
            var client = new HttpClient();
            var stringBuilder = new StringBuilder();

            var returnTextOnly = isTextOnly?.ToLowerInvariant() == "textonly";

            icao = icao.ToUpperInvariant();

            if (string.IsNullOrEmpty(icao) || icao.Length < 3 || icao.Length > 4)
            {
                return new JsonResult(new ApiError(""));
            }

            var countryCodes = new[] { 'K', 'P', 'T' };
            if (icao.Length == 4 && !countryCodes.Any(x => icao.StartsWith(x)))
            {
                return new JsonResult(new ApiError("Please provide a valid American ICAO-Code (KXXX, PANC, PHNL, or TJSJ)"));
            }

            if (icao.Length == 3)
            {
                var icaoCode = icao switch
                {
                    "ANC" or "HNL" => $"P{icao}",
                    "JSJ" => $"T{icao}",
                    _ => $"K{icao}",
                };

                if (!DAtisAirports.Any(x => x == icaoCode))
                {
                    return new JsonResult(new ApiError("Your ICAO does not have a D-ATIS"));
                }

                var concatAtis = client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icaoCode}").Result;

                if (concatAtis == null)
                {
                    return new JsonResult(new ApiError("Error fetching D-ATIS"));
                }

                if (returnTextOnly)
                {
                    concatAtis.ForEach(x => stringBuilder.AppendLine(x.Datis));

                    return new JsonResult(stringBuilder.ToString());
                }

                return new JsonResult(concatAtis, Options.JsonOptions);
            }

            if (!DAtisAirports.Any(x => x == icao))
            {
                return new JsonResult(new ApiError("Your ICAO does not have a D-ATIS"));
            }

            var atis = client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icao}").Result;

            if (atis == null || atis.Count == 0)
            {
                return new JsonResult(new ApiError("No D-ATIS found for your airpot"));
            }

            if (returnTextOnly)
            {
                atis.ForEach(x => stringBuilder.AppendLine(x.Datis));

                return new JsonResult(stringBuilder.ToString(), Options.JsonOptions);
            }

            return new JsonResult(atis, Options.JsonOptions);
        }
    }
}
