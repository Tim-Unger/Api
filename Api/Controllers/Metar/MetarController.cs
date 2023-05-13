using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using MetarSharp;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using MetarSharp.Methods.Download;
using System.Text.Json;
using System.Security.AccessControl;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using Api.Controllers.Metar;
using System.Diagnostics.Metrics;
using Swashbuckle.Examples;

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
        /// <summary>
        /// Get the metar of an ICAO as a simple string (uses the Vatsim-Metar-Service)
        /// </summary>
        /// <param name="icao"></param>
        /// <returns></returns>
        [HttpGet("/metar/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("string")]
        public string Get(string icao = null)
        {
            if (icao == null || icao.Length != 4)
            {
                return "ICAO code provided is invalid or was not given";
            }

            return DownloadMetar.FromVatsimSingle(icao);
        }

        /// <summary>
        /// Get the metar of an ICAO and decodes it into JSON (uses the Vatsim-Metar-Service for the Metar and MetarSharp for the decoding)
        /// You can also only use a country or region code (e.g. EG/Y/K), which will return a list of all decoded metars from that region
        /// </summary>
        /// <remarks>
        /// Example Output (Only :
        /// </remarks>
        /// <param name="icao"></param>
        /// <returns></returns>
        [HttpGet("/metar/{icao}/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAndDecode(string icao = null)
        {
            if (icao == null)
            {
                return Json("No ICAO provided");
            }

            var metars = new List<MetarSharp.Metar>();

            DownloadMetar.FromVatsimMultiple(icao).ForEach(x => metars.Add(ParseMetar.FromString(x)));
            
            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(Json(GetMetar(x))));
            var resultsFiltered = jsonResults.Select(x => x.Value);

            return Json(resultsFiltered);
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

        [HttpGet("/metar/{icao}/decode/{type}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAndDecodePartially(string icao = null, string type = null)
        {
            if (icao == null)
            {
                return Json("No ICAO provided");
            }

            MetarType metarType = type.ToLower() switch
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
                return Json("Metar Type was not valid");
            }

            var metars = new List<MetarSharp.Metar>();
            DownloadMetar.FromVatsimMultiple(icao).ForEach(x => metars.Add(ParseMetar.FromString(x)));

            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(GetMetarType(x, metarType)));
            var resultsFiltered = jsonResults.Select(x => x.Value);

            return Json(resultsFiltered);
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
