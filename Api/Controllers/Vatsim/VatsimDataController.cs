using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers.Vatsim;
using Microsoft.AspNetCore.Cors;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.Json;
using static Json.Json;
using System.Data;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Api.Controllers
{
    public class Data
    {
        public static List<Vatsim.Server> Servers { get; set; }
    }

    [Route("api")]
    internal class VatsimDataController : Microsoft.AspNetCore.Mvc.Controller
    {
        private static readonly List<string> AllowedTypes = new()
        {
            "general",
            "pilots",
            "controllers",
            "atis",
            "servers",
            "prefiles",
            "facilities",
            "ratings",
            "pilot_ratings"
        };

        [HttpGet("/vatsim/status")]
        public JsonResult GetStatus()
        {
            Vatsim.GetStatus.Status();

            return Json(Data.Servers);
        }

        /// <summary>
        /// Gets the entire Vatsim Data-Feed
        /// </summary>
        /// <returns></returns>
        [HttpGet("/vatsim/data")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult GetaAllData()
        {
            try
            {
                return Json(Vatsim.GetData.GetVatsimData().Result);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet("/vatsim/data/{type}/{callsign?}/{traffictype?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult GetData(
            string type = null,
            string callsign = null,
            string trafficType = null
        )
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var convertedType = type.ToLower();

            if (!AllowedTypes.Any(x => x == convertedType))
            {
                return Json("Use Use on of the following types: general, pilots, controllers, atis, servers, prefiles, facilities, ratings, pilot_ratings");
            }

            string vatsimData = null;

            try
            {
                vatsimData = Vatsim.GetData.GetVatsimData().Result;
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            var data = JsonSerializer.Deserialize<Rootobject>(vatsimData)!;

            if (callsign == null)
            {
                if (string.IsNullOrEmpty(vatsimData))
                {
                    return Json("Error with the Vatsim Data");
                }

                return convertedType switch
                {
                    "general" => Json(data.general),
                    "pilots" => Json(data.pilots),
                    "controllers" => Json(data.controllers),
                    "atis" => Json(data.atis),
                    "servers" => Json(data.servers),
                    "prefiles" => Json(data.prefiles),
                    "facilities" => Json(data.facilities),
                    "ratings" => Json(data.ratings),
                    "pilot_ratings" => Json(data.pilot_ratings),
                    _ => Json("Type not recognized")
                };
            }

            var allowedTypesWithCallsign = new string[]
                {
                    "pilots",
                    "controllers",
                    "atis",
                    "prefiles"
                };

            if (!allowedTypesWithCallsign.Any(x => x == convertedType))
            {
                return Json("You can only use the callsign-option with the following types: pilots, controllers, atis, prefiles");
            }

            var isCid = int.TryParse(callsign, out var cid);

            if (isCid)
            {
                return convertedType switch
                {
                    "pilots" => Json(data.pilots.Where(x => x.cid == cid).ToList()),
                    "controllers" => Json(data.controllers.Where(x => x.cid == cid).ToList()),
                    "atis" => Json(data.atis.Where(x => x.cid == cid).ToList()),
                    "prefiles" => Json(data.prefiles.Where(x => x.cid == cid).ToList()),
                    _ => Json("")
                };
            }

            var isCallsign = callsign.Contains('_');
            var convertedCallsign = callsign.ToUpper();

            if (isCallsign)
            {
                return convertedType switch
                {
                    "pilots" => Json(data.pilots.Where(x => x.callsign == convertedCallsign).ToString()),
                    "controllers" => Json(data.controllers.Where(x => x.callsign == convertedCallsign).ToString()),
                    "atis" => Json(data.atis.Where(x => x.callsign == convertedCallsign).ToString()),
                    "prefiles" => Json(data.prefiles.Where(x => x.callsign == convertedCallsign).ToString()),
                    _ => Json("")
                };
            }

            if (
                callsign.Length <= 4
                && (convertedType == "pilots" || convertedType == "controllers")
            )
            {
                string result = null;

                if (convertedType == "controllers")
                {
                    var allControllers =
                        data.controllers.Where(
                            x => x.callsign.ToLower()
                            .StartsWith(callsign.ToLower()))
                            .ToList();

                    if (allControllers.Count == 0)
                    {
                        return Json("No controllers found");
                    }

                    return Json(allControllers);
                }

                var allowedTrafficTypes = new string[]
                {
                "inbounds",
                "outbounds",
                };

                if (trafficType == null)
                {
                    var allPilotsList = data.pilots
                        .Where(x => x.flight_plan != null)
                        .Where(x => x.flight_plan.departure.StartsWith(callsign.ToLower())
                            || x.flight_plan.arrival.StartsWith(callsign.ToLower()))
                        .ToList();

                    if (allPilotsList.Count == 0)
                    {
                        return Json("No Pilots found");
                    }

                    return Json(allPilotsList);
                }
            }

            var convertedTrafficType = trafficType.ToLower();
            var allPilots = new List<Pilot>();

            if (convertedTrafficType == "inbounds")
            {
                allPilots = data.pilots.Where(x => x.flight_plan != null)
                    .Where(x => x.flight_plan.arrival.ToLower().StartsWith(callsign.ToLower()))
                    .ToList();

                if (allPilots.Count == 0)
                {
                    return Json("No Pilots inbound to this airport found");
                }

                return Json(allPilots);
            }

            allPilots = data.pilots.Where(x => x.flight_plan != null).Where(x => x.flight_plan.departure.ToLower().StartsWith(callsign.ToLower())).ToList();

            if(allPilots.Count == 0)
            {
                return Json("No Pilots outbound from this airport found");
            }

            return Json(allPilots);
        }
    }
}
