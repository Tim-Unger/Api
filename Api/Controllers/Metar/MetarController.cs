﻿using MetarSharp;
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
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Raw Metar(s)",
                        Params = icao
                    }
                );

                return "ICAO code provided is invalid or was not given";
            }

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Raw Metar(s)",
                    Params = icao
                }
            );

            return string.Join(Environment.NewLine, DownloadMetar.FromVatsimMultiple(icao));
        }

        [HttpGet("/metar/decode")]
        [Produces("application/json")]
        public async Task<JsonResult> GetAndDecodeBody()
        {
            var bodyStream = Request.Body;

            var streamReader = new StreamReader(bodyStream);

            var body = await streamReader.ReadToEndAsync();

            if (body is null || string.IsNullOrWhiteSpace(body))
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Decode Metar from body",
                    }
                );

                return Json(new ApiError("Please provide a body"), Options.JsonOptions);
            }

            try
            {
                var decodedMetar = ParseMetar.FromString(body);

                var metarJson = GetMetar(decodedMetar);

                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Success,
                        ApiRequestType = "GET",
                        RequestName = "Decode Metar from body",
                        Params = body
                    }
                );

                return new JsonResult(metarJson, Options.JsonOptions);
            }
            catch
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Decode Metar from body",
                        Params = body
                    }
                );

                return Json(new ApiError("Please provide a valid metar"), Options.JsonOptions);
            }
        }

        /// <summary>
        /// Get the metar of an ICAO and decodes it into JSON
        /// </summary>
        /// <remarks>
        /// (uses https://metar.vatsim.net/ for the Metar and http://metar.tim-u.me/ for the decoding)
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
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Decode Metar(s)",
                        Params = icao
                    }
                );

                return Json(new ApiError("No ICAO provided"), Options.JsonOptions);
            }

            var metars = DownloadMetar.FromVatsimMultiple(icao).ParseMetars();

            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(Json(GetMetar(x))));

            //Since GetMetar returns an object the Value of each item needs to be selected)
            var resultsFiltered = jsonResults.Select(x => x.Value);

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Decode Metar(s)",
                    Params = icao
                }
            );

            return Json(resultsFiltered, Options.JsonOptions);
        }

        //This is the only way to return everything as one json
        private static object GetMetar(MetarSharp.Metar metar) =>
            new
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
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Decode Type",
                        Params = $"{icao}, {type}"
                    }
                );

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
                "additionalinformation"
                or "info"
                or "additional"
                  => MetarType.AdditionalInformation,
                _ => MetarType.Error
            };

            if (metarType == MetarType.Error)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Decode Type",
                        Params = $"{icao}, {type}"
                    }
                );

                return Json(
                    new ApiError("Metar Type was not valid or not given"),
                    Options.JsonOptions
                );
            }

            var metars = new List<MetarSharp.Metar>();
            DownloadMetar
                .FromVatsimMultiple(icao)
                .ForEach(x => metars.Add(ParseMetar.FromString(x)));

            var jsonResults = new List<JsonResult>();
            metars.ForEach(x => jsonResults.Add(GetMetarType(x, metarType)));
            var resultsFiltered = jsonResults.Select(x => x.Value);

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Decode Type",
                    Params = $"{icao}, {type}"
                }
            );

            return Json(resultsFiltered, Options.JsonOptions);
        }

        /// <summary>
        /// Get a readable report for a netar from an ICAO-Code
        /// </summary>
        /// <param name="icao"></param>
        /// <returns></returns>
        [HttpGet("/metar/{icao}/readablereport")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetAndDecodeReadableReport(string icao)
        {
            if (icao == null)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Metar Readable Report",
                        Params = icao
                    }
                );

                return "No ICAO provided";
            }

            if (icao.Length != 4)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Metar Readable Report",
                        Params = icao
                    }
                );

                return "ICAO Length must be 4 letters";
            }

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Metar Readable Report",
                    Params = icao
                }
            );

            return ParseMetar.FromString(DownloadMetar.FromVatsimSingle(icao)).ReadableReport ?? "";
        }

        private JsonResult GetMetarType(MetarSharp.Metar metar, MetarType metarType) =>
            metarType switch
            {
                MetarType.Raw => Json(new { metar.Airport, metar.MetarRaw }),
                MetarType.Airport => Json(new { metar.Airport }),
                MetarType.ReportingTime => Json(metar.Airport, metar.ReportingTime),
                MetarType.AutomatedReport => Json(new { metar.Airport, metar.IsAutomatedReport }),
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
                MetarType.AdditionalInformation
                  => Json(new { metar.Airport, metar.AdditionalInformation }),
                _ => Json("") //You should not be able to get here
            };
    }
}
