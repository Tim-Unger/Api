using MetarSharp;
using MetarSharp.Downloader;

namespace Api.Controllers.Metar
{
    internal enum MetarType
    {
        Raw,
        Airport,
        ReportingTime,
        AutomatedReport,
        Wind,
        Visibility,
        RunwayVisibility,
        Weather,
        Clouds,
        Temperature,
        Pressure,
        Trends,
        RunwayCondition,
        ReadableReport,
        AdditionalInformation,
        Error
    }

    [Route("api")]
    [ApiController]
    public class MetarController : Controller
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Get the metar of an ICAO as a simple string
        /// </summary>
        /// <param name="icao"></param>
        /// <returns>The Raw Metar as a string</returns>
        [HttpGet("/metar/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string Get(string icao)
        {
            if (icao == null || icao.Length < 1 || icao.Length > 4)
            {
                return "ICAO code provided is invalid or was not given";
            }

            return string.Join(Environment.NewLine, DownloadMetar.FromVatsimMultiple(icao));
        }

        /// <summary>
        /// Get the metar of an ICAO and decodes it into JSON 
        /// </summary>
        /// <remarks>
        /// (uses the Vatsim-Metar-Service for the Metar and MetarSharp for the decoding)
        /// You can also only use a country or region code (e.g. EG/Y/K), which will return a list of all decoded metars from that region
        /// </remarks>
        /// <param name="icao"></param>
        /// <returns>Json Array of the requested Metar(s)</returns>
        [HttpGet("/metar/{icao}/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAndDecode(string icao)
        {
            if (icao == null)
            {
                return Json("No ICAO provided");
            }

            var metars = DownloadMetar.FromVatsimMultiple(icao).ParseMetars();
            
            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(Json(GetMetar(x))));
            //Since GetMetar returns an object the Value of each item needs to be selected)
            var resultsFiltered = jsonResults.Select(x => x.Value);

            return Json(resultsFiltered, _jsonOptions);
        }

        //This is the only way to return everything as one json
        private static object GetMetar(MetarSharp.Metar metar) => new
        {
            metar.MetarRaw,
            metar.Airport,
            metar.ReportingTime,
            metar.IsAutomatedReport,
            metar.Wind,
            metar.Visibility,
            metar.RunwayVisibilities,
            metar.Weather,
            metar.Clouds,
            metar.Temperature,
            metar.Pressure,
            metar.Trends,
            metar.RunwayConditions,
            metar.ReadableReport,
            metar.AdditionalInformation
        };

        /// <summary>
        /// Get the Metar of an ICAO and decode it, but only returns part of the decoded Metar the user wants.
        /// </summary>
        /// <remarks>
        ///  You can also only use a country or region code (e.g. EG/Y/K), which will return a list of all decoded metars from that region
        /// You can use:
        /// Raw Metar, Airport, Reporting Time, Whether the Report is automated, Wind, Visibility, Runway Visibility, Weather, Clouds, Temperature, Pressure, Trend, Runway Condition, Readable Report, Additional Information
        /// </remarks>
        /// <example>
        /// </example>
        /// <param name="icao"></param>
        /// <param name="type"></param>
        /// <returns>Json Array of the requested Metar Part(s)</returns>
        [HttpGet("/metar/{icao}/decode/{type}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAndDecodePartially(string icao, string type)
        {
            if (icao == null)
            {
                return Json("No ICAO provided");
            }

            var metarType = type.ToLower() switch
            {
                "raw" or "metarraw" => MetarType.Raw,
                "airport" => MetarType.Airport,
                "reportingtime" or "time" => MetarType.ReportingTime,
                "automated" or "isautomated" or "automatedreport" => MetarType.AutomatedReport,
                "wind" => MetarType.Wind,
                "visibility" => MetarType.Visibility,
                "runwayvisibility" or "rvr" or "runwayvisibilities" => MetarType.RunwayVisibility,
                "weather" => MetarType.Weather,
                "clouds" or "cloud" => MetarType.Clouds,
                "temperature" => MetarType.Temperature,
                "pressure" => MetarType.Pressure,
                "trends" or "trend" => MetarType.Trends,
                "runwaycondition" or "runwayconditions" => MetarType.RunwayCondition,
                "readable" or "readablereport" or "decoded" => MetarType.ReadableReport,
                "additionalinformation" or "info" or "additional" => MetarType.AdditionalInformation,
                _ => MetarType.Error
            };

            if (metarType == MetarType.Error)
            {
                return Json("Metar Type was not valid or not given");
            }

            var metars = new List<MetarSharp.Metar>();
            DownloadMetar.FromVatsimMultiple(icao).ForEach(x => metars.Add(ParseMetar.FromString(x)));

            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(GetMetarType(x, metarType)));
            var resultsFiltered = jsonResults.Select(x => x.Value);

            return Json(resultsFiltered);
        }

        [HttpGet("/metar/{icao}/readablereport")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetAndDecodeReadableReport(string icao)
        {
            if (icao == null)
            {
                return "No ICAO provided";
            }

            if (icao.Length != 4)
            {
                return "ICAO Length must be 4 letters";
            }

            return ParseMetar.FromString(DownloadMetar.FromVatsimSingle(icao)).ReadableReport ?? "";
        }

        private JsonResult GetMetarType(MetarSharp.Metar metar, MetarType metarType) => metarType switch
        {

            MetarType.Raw => Json(new {metar.Airport, metar.MetarRaw }),
            MetarType.Airport => Json(new {metar.Airport }),
            MetarType.ReportingTime => Json(metar.Airport, metar.ReportingTime),
            MetarType.AutomatedReport => Json(new {metar.Airport, metar.IsAutomatedReport }),
            MetarType.Wind => Json(new { metar.Airport, metar.Wind }),
            MetarType.Visibility => Json(new { metar.Airport, metar.Visibility }),
            MetarType.RunwayVisibility => Json(new { metar.Airport, metar.RunwayVisibilities }),
            MetarType.Weather => Json(new { metar.Airport, metar.Weather }),
            MetarType.Clouds => Json(new { metar.Airport, metar.Clouds }),
            MetarType.Temperature => Json(new { metar.Airport, metar.Temperature }),
            MetarType.Pressure => Json(new { metar.Airport, metar.Pressure }),
            MetarType.Trends => Json(new { metar.Airport, metar.Trends }),
            MetarType.RunwayCondition => Json(new { metar.Airport, metar.RunwayConditions }),
            MetarType.ReadableReport => Json(new { metar.Airport, metar.ReadableReport }),
            MetarType.AdditionalInformation => Json(new { metar.Airport, metar.AdditionalInformation }),
            _ => Json("") //You should not be able to get here
        };
    }
}