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
    public class MetarController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet("/metar/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string Get(string icao = null)
        {
            if (icao == null)
            {
                return "No ICAO provided";
            }

            return DownloadMetar.FromVatsimSingle(icao);
        }

        [HttpGet("/metar/{icao}/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAndDecode(string icao = null)
        {
            if (icao == null)
            {
                return Json("No ICAO provided");
            }

            var metar = ParseMetar.FromString(DownloadMetar.FromVatsimSingle(icao));
            var result = new
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

            return Json(result);
        }

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

            if(metarType == MetarType.Error)
            {
                return Json("Metar Type was not valid");
            }

            var metar = ParseMetar.FromString(DownloadMetar.FromVatsimSingle(icao));

            return metarType switch
            {
                MetarType.Raw => Json(new { metar.MetarRaw }),
                MetarType.Airport => Json(new { metar.Airport }),
                MetarType.ReportingTime => Json(metar.ReportingTime),
                MetarType.AutomatedReport => Json(new { metar.IsAutomatedReport }),
                MetarType.Wind => Json(metar.Wind),
                MetarType.Visibility => Json(metar.Visibility),
                MetarType.RunwayVisibility => Json(metar.RunwayVisibilities),
                MetarType.Weather => Json(metar.Weather),
                MetarType.Clouds => Json(metar.Clouds),
                MetarType.Temperature => Json(metar.Temperature),
                MetarType.Pressure => Json(metar.Pressure),
                MetarType.Trends => Json(metar.Trends),
                MetarType.RunwayCondition => Json(metar.RunwayConditions),
                MetarType.ReadableReport => Json(new { metar.ReadableReport }),
                MetarType.AdditionalInformation => Json(metar.AdditionalInformation),
                _ => Json("") //You should not be able to get here
            };
        }
    }
}
