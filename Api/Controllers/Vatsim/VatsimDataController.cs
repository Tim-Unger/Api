using Api.Controllers.Vatsim;
using Microsoft.AspNetCore.Cors;
using System.Net.NetworkInformation;
using static Json.Json;
using System.Data;
using Server = Api.Controllers.Vatsim.Server;
using Api.Controllers.Airac;
using Api;

namespace Api.Controllers
{
    [Route("api")]
    public class VatsimController : Microsoft.AspNetCore.Mvc.Controller
    {
        private static readonly List<string> _allowedTypes =
            new()
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

        /// <summary>
        /// Gets the Vatsim-Status
        /// </summary>
        /// <returns></returns>
        [HttpGet("/vatsim/status")]
        public async Task<JsonResult> GetStatus()
        {
            var serverLinks = new List<string>();

            var client = new HttpClient();
            var data = await client.GetStringAsync("https://status.vatsim.net/status.json");

            var root = JsonSerializer.Deserialize<Status>(data);

            root!.data.servers.ToList().ForEach(serverLinks.Add);

            var random = new Random();

            var randomServerIndex = random.Next(0, serverLinks.Count);

            var servers = await client.GetStringAsync(serverLinks[randomServerIndex]);

            var vatsimServer =
                JsonSerializer.Deserialize<List<VatsimServer>>(servers) ?? throw new Exception();

            var vatsimData = await client.GetFromJsonAsync<Json.Json.VatsimData>(
                "https://data.vatsim.net/v3/vatsim-data.json"
            );

            if (vatsimData == null)
            {
                return Json(new ApiError("Vatsim-Data could not be read"), Options.JsonOptions);
            }

            var serverCountList = new List<string>();

            vatsimData.Pilots.ForEach(x => serverCountList.Add(x.server));
            vatsimData.Controllers.ForEach(x => serverCountList.Add(x.server));
            vatsimData.Atis.ForEach(x => serverCountList.Add(x.server));

            //Counts how often each server appears and then sorts them by largest first
            var countServers = serverCountList
                .GroupBy(x => x)
                .Select(x => new { Value = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var serversList = new List<Server>
            {
                //new Server() { Name = "Vatsim Website", Connections = null, Operational = IsServerOperational("https://vatsim.net") },
                //new Server() { Name = "Vatsim Forum", Connections = null, Operational = IsServerOperational("https://forum.vatsim.net") },
                //new Server() { Name = "Vatsim Metar", Connections = null, Operational = IsServerOperational("https://metar.vatsim.net") },
                //new Server() { Name = "Vatsim Data", Connections = null, Operational = IsServerOperational("https://status.vatsim.net/status.json") }
            };

            //Ping the each server once and create a list with the results
            foreach (var server in vatsimServer)
            {
                var pingServer = new Ping();
                PingReply pingServerReply = pingServer.Send(server.hostname_or_ip);

                var isOperational = pingServerReply.Status == IPStatus.Success;

                var newServer = new Vatsim.Server
                {
                    Name = server.name,
                    Operational = isOperational,
                    Connections = serverCountList.Count
                };
                serversList.Add(newServer);
            }

            //Adds the count of each server to the list (can't ping these servers themselves since the automatic server exists)
            countServers.ForEach(
                x =>
                    serversList.Add(
                        new Server()
                        {
                            Connections = x.Count,
                            Name = x.Value,
                            Operational = true
                        }
                    )
            );

            return Json(serversList, Options.JsonOptions);
        }

        //private static bool IsServerOperational(string ip)
        //{
        //    var pingServer = new Ping();
        //    PingReply pingServerReply = pingServer.Send(ip);

        //    return pingServerReply.Status == IPStatus.Success;
        //}

        /// <summary>
        /// Gets the entire Vatsim Data-Feed
        /// </summary>
        /// <returns></returns>
        [HttpGet("/vatsim/data")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> GetaAllData()
        {
            try
            {
                var dataRaw = await Vatsim.GetData.GetVatsimData();

                var data = JsonSerializer.Deserialize<VatsimData>(
                    dataRaw,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );

                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Success,
                        ApiRequestType = "GET",
                        RequestName = "Vatsim Data"
                    }
                );

                return Json(data, Options.JsonOptions);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, Options.JsonOptions);
            }
        }

        [HttpGet("/vatsim/data/{type}/{callsign?}/{traffictype?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult GetData(string type, string? callsign = null, string? trafficType = null)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var convertedType = type.ToLower();

            if (!_allowedTypes.Any(x => x == convertedType))
            {
                return Json(
                    "Use Use on of the following types: general, pilots, controllers, atis, servers, prefiles, facilities, ratings, pilot_ratings",
                    Options.JsonOptions
                );
            }

            string? vatsimData = null;

            try
            {
                vatsimData = Vatsim.GetData.GetVatsimData().Result;
            }
            catch (Exception ex)
            {
                return Json(ex.Message, Options.JsonOptions);
            }

            var data = JsonSerializer.Deserialize<VatsimData>(vatsimData)!;

            if (callsign == null)
            {
                if (string.IsNullOrEmpty(vatsimData))
                {
                    return Json("Error with the Vatsim Data", Options.JsonOptions);
                }

                return convertedType switch
                {
                    "general" => Json(data.General, Options.JsonOptions),
                    "pilots" => Json(data.Pilots, Options.JsonOptions),
                    "controllers" => Json(data.Controllers, Options.JsonOptions),
                    "atis" => Json(data.Atis, Options.JsonOptions),
                    "servers" => Json(data.Servers, Options.JsonOptions),
                    "prefiles" => Json(data.Prefiles, Options.JsonOptions),
                    "facilities" => Json(data.Facilities, Options.JsonOptions),
                    "ratings" => Json(data.Ratings, Options.JsonOptions),
                    "pilot_ratings"
                    or "pilotratings"
                      => Json(data.PilotRatings, Options.JsonOptions),
                    _ => Json("Type not recognized", Options.JsonOptions)
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
                return Json(
                    "You can only use the callsign-option with the following types: pilots, controllers, atis, prefiles",
                    Options.JsonOptions
                );
            }

            if (int.TryParse(callsign, out var cid))
            {
                return convertedType switch
                {
                    "pilots"
                      => Json(data.Pilots.Where(x => x.cid == cid).ToList(), Options.JsonOptions),
                    "controllers"
                      => Json(
                          data.Controllers.Where(x => x.cid == cid).ToList(),
                          Options.JsonOptions
                      ),
                    "atis"
                      => Json(data.Atis.Where(x => x.cid == cid).ToList(), Options.JsonOptions),
                    "prefiles"
                      => Json(data.Prefiles.Where(x => x.cid == cid).ToList(), Options.JsonOptions),
                    _ => Json("", Options.JsonOptions)
                };
            }

            if (callsign.Contains('_'))
            {
                var convertedCallsign = callsign.ToUpper();

                return convertedType switch
                {
                    "pilots"
                      => Json(
                          data.Pilots.Where(x => x.callsign == convertedCallsign).ToString(),
                          Options.JsonOptions
                      ),
                    "controllers"
                      => Json(
                          data.Controllers.Where(x => x.callsign == convertedCallsign).ToString(),
                          Options.JsonOptions
                      ),
                    "atis"
                      => Json(
                          data.Atis.Where(x => x.callsign == convertedCallsign).ToString(),
                          Options.JsonOptions
                      ),
                    "prefiles"
                      => Json(
                          data.Prefiles.Where(x => x.callsign == convertedCallsign).ToString(),
                          Options.JsonOptions
                      ),
                    _ => Json("", Options.JsonOptions)
                };
            }

            if (
                callsign.Length <= 4
                && (convertedType == "pilots" || convertedType == "controllers")
            )
            {
                if (convertedType == "controllers")
                {
                    var allControllers = data.Controllers
                        .Where(x => x.callsign.ToLower().StartsWith(callsign.ToLower()))
                        .ToList();

                    if (allControllers.Count == 0)
                    {
                        return Json("No controllers found", Options.JsonOptions);
                    }

                    return Json(allControllers, Options.JsonOptions);
                }

                var allowedTrafficTypes = new string[] { "inbounds", "outbounds", };

                if (trafficType == null)
                {
                    var allPilotsList = data.Pilots
                        .Where(x => x.flight_plan != null)
                        .Where(
                            x =>
                                x.flight_plan.departure.StartsWith(callsign.ToLower())
                                || x.flight_plan.arrival.StartsWith(callsign.ToLower())
                        )
                        .ToList();

                    if (allPilotsList.Count == 0)
                    {
                        return Json("No Pilots found", Options.JsonOptions);
                    }

                    return Json(allPilotsList, Options.JsonOptions);
                }

                var convertedTrafficType = trafficType.ToLower();
                var allPilots = new List<Pilot>();

                if (convertedTrafficType == "inbounds")
                {
                    allPilots = data.Pilots
                        .Where(x => x.flight_plan != null)
                        .Where(x => x.flight_plan.arrival.ToLower().StartsWith(callsign.ToLower()))
                        .ToList();

                    if (allPilots.Count == 0)
                    {
                        return Json("No Pilots inbound to this airport found", Options.JsonOptions);
                    }

                    return Json(allPilots, Options.JsonOptions);
                }

                allPilots = data.Pilots
                    .Where(x => x.flight_plan != null)
                    .Where(x => x.flight_plan.departure.ToLower().StartsWith(callsign.ToLower()))
                    .ToList();

                if (allPilots.Count == 0)
                {
                    return Json("No Pilots outbound from this airport found", Options.JsonOptions);
                }

                return Json(allPilots, Options.JsonOptions);
            }

            return Json("", Options.JsonOptions);
        }
    }
}
