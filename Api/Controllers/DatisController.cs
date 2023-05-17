using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class DatisController : Controller
    {
        private static readonly List<string> DAtisAirports = new()
    {
        "KABQ",
        "KADW",
        "KALB",
        "KATL",
        "KAUS",
        "KBDL",
        "KBNA",
        "KBOI",
        "KBOS",
        "KBUF",
        "KBUR",
        "KBWI",
        "KCHS",
        "KCLE",
        "KCLT",
        "KCMH",
        "KCVG",
        "KDAL",
        "KDCA",
        "KDEN",
        "KDFW",
        "KDTW",
        "KELP",
        "KEWR",
        "KFLL",
        "KGSO",
        "KHOU",
        "KHPN",
        "KIAD",
        "KIAH",
        "KIND",
        "KJAX",
        "KJFK",
        "KLAS",
        "KLGA",
        "KLIT",
        "KMCI",
        "KMCO",
        "KMDW",
        "KMEM",
        "KMIA",
        "KMKE",
        "KMSP",
        "KMSY",
        "KOAK",
        "KOKC",
        "KOMA",
        "KONT",
        "KORD",
        "KPBI",
        "KPDX",
        "KPHL",
        "KPHX",
        "KPIT",
        "KPVD",
        "KRDU",
        "KRNO",
        "KRSW",
        "KSAN",
        "KSAT",
        "KSDF",
        "KSEA",
        "KSFO",
        "KSJC",
        "KSLC",
        "KSMF",
        "KSNA",
        "KSTL",
        "KTEB",
        "KTPA",
        "KTUL",
        "KVNY",
        "PANC",
        "PHNL",
        "TJSJ"
    };

        internal class DAtis
        {
            [JsonPropertyName("airport")]
            public string Airport { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("datis")]
            public string Datis { get; set; }
        }

        /// <summary>
        /// Get all supported D-ATIS Airports
        /// </summary>
        /// <returns></returns>
        [HttpGet("/datis/airports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetAirports()
        {
            return string.Join(Environment.NewLine, DAtisAirports);
        }

        /// <summary>
        /// Get the D-Atis for an American Airport
        /// </summary>
        /// <remarks>
        /// Please use /datis/airports to see a list of supported airports.
        /// Append /textonly to only get the Text of the D-ATIS
        /// </remarks>
        /// <param name="icao">the four or three letter ICAO of the Airport</param>
        /// <param name="textOnly">whether you would like the D-ATIS text only</param>
        /// <returns></returns>
        [HttpGet("/datis/{icao}/{textOnly?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get(string icao, string? textOnly = null)
        {
            var client = new HttpClient();

            var returnTextOnly = textOnly?.ToLowerInvariant() == "textonly";

            if (string.IsNullOrEmpty(icao) || icao.Length < 3 || icao.Length > 4)
            {
                return Json("Please provide a valid ICAO (JKF or KJFK)");
            }

            var countryCodes = new[] { 'K', 'P', 'T' };
            if (icao.Length == 4 && !countryCodes.Any(x => icao.ToUpper().StartsWith(x)))
            {
                return Json("Please provide a valid American ICAO-Code (KXXX, PANC, PHNL, or TJSJ)");
            }

            if (icao.Length == 3)
            {
                var icaoCode = icao.ToUpper() switch
                {
                    "ANC" or "HNL" => $"P{icao.ToUpper()}",
                    "JSJ" => $"T{icao.ToUpper()}",
                    _ => $"K{icao.ToUpper()}",
                };

                if (!DAtisAirports.Any(x => x == icaoCode))
                {
                    return Json("Your ICAO does not have a D-ATIS");
                }

                var concatAtis = await client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icaoCode}");

                if (concatAtis == null)
                {
                    return Json("Error fetching D-ATIS");
                }

                if (returnTextOnly)
                {
                    var stringBuilder = new StringBuilder();

                    concatAtis.ForEach(x => stringBuilder.AppendLine(x.Datis));

                    return Json(stringBuilder.ToString());
                }

                return Json(concatAtis);
            }

            if (!DAtisAirports.Any(x => x == icao.ToUpper()))
            {
                return Json("Your ICAO does not have a D-ATIS");
            }

            var atis = await client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icao}");

            if (atis == null || atis.Count == 0)
            {
                return Json("No D-ATIS found for your airpot");
            }

            if (returnTextOnly)
            {
                var stringBuilder = new StringBuilder();

                atis.ForEach(x => stringBuilder.AppendLine(x.Datis));

                return Json(stringBuilder.ToString());
            }

            return Json(atis);
        }
    }
}
