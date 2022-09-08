using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers.Vatsim;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using static Json.Json;

namespace Api.Controllers
{
    public class Data
    {
        public static List<Vatsim.Server> Servers { get; set; }
    }

    [Route("/Vatsim")]
    public class VatsimDataController : Microsoft.AspNetCore.Mvc.Controller
    {
        List<string> AllowedTypes = new List<string>
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

        [HttpGet("status")]
        public JsonResult GetStatus()
        {
            Vatsim.GetStatus.Status();

            return Json(Data.Servers);
        }

        [HttpGet("data")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult GetaAllData()
        {
            string VatsimData = null;
            try
            {
                VatsimData = Vatsim.GetData.GetVatsimData();
                Rootobject Raw = JsonConvert.DeserializeObject<Rootobject>(VatsimData);
                return Json(Raw);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet("data/{type}/{callsign?}/{traffictype?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult GetData(
            string Type = null,
            string Callsign = null,
            string TrafficType = null
        )
        {
            string ConvertedType = Type.ToLower();

            if (AllowedTypes.Contains(ConvertedType))
            {
                string VatsimData = null;

                try
                {
                    VatsimData = Vatsim.GetData.GetVatsimData();
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
                if (Callsign == null)
                {
                    if (!string.IsNullOrEmpty(VatsimData))
                    {
                        Rootobject Data = JsonConvert.DeserializeObject<Rootobject>(VatsimData);

                        switch (ConvertedType)
                        {
                            case "general":
                                return Json(Data.general);
                            case "pilots":
                                return Json(Data.pilots);
                            case "controllers":
                                return Json(Data.controllers);
                            case "atis":
                                return Json(Data.atis);
                            case "servers":
                                return Json(Data.servers);
                            case "prefiles":
                                return Json(Data.prefiles);
                            case "facilities":
                                return Json(Data.facilities);
                            case "ratings":
                                return Json(Data.ratings);
                            case "pilot_ratings":
                                return Json(Data.pilot_ratings);
                        }
                    }
                    return null;
                }
                else
                {
                    string[] AllowedTypesWithCallsign = new string[]
                    {
                        "pilots",
                        "controllers",
                        "atis",
                        "prefiles"
                    };
                    if (AllowedTypesWithCallsign.Contains(ConvertedType))
                    {
                        Rootobject Data = JsonConvert.DeserializeObject<Rootobject>(VatsimData);
                        bool IsCid = int.TryParse(Callsign, out int Cid);

                        if (IsCid)
                        {
                            string Result = null;

                            
                            switch (ConvertedType)
                            {
                                case "pilots":
                                    Result = Data.pilots.Where(x => x.cid == Cid).ToString();

                                    break;
                                case "controllers":
                                    Result = Data.controllers.Where(x => x.cid == Cid).ToString();
                                    break;
                                case "atis":
                                    Result = Data.atis.Where(x => x.cid == Cid).ToString();
                                    break;
                                case "prefiles":
                                    Result = Data.prefiles.Where(x => x.cid == Cid).ToString();
                                    break;
                            }

                            if (Result != null && Result != "[]")
                            {
                                return Json(Result);
                            }
                            else
                            {
                                VatsimError InnerError = new VatsimError()
                                {
                                    Error = "Controller not found"
                                };
                                return Json(InnerError);
                            }
                        }
                        else
                        {
                            bool IsCallsign = Callsign.Contains('_');
                            string ConvertedCallsign = Callsign.ToUpper();
                            if (IsCallsign)
                            {
                                string Result = null;
                                switch (ConvertedType)
                                {
                                    case "pilots":
                                        Result = Data.pilots
                                            .Where(x => x.callsign == ConvertedCallsign)
                                            .ToString();
                                        break;
                                    case "controllers":
                                        Result = Data.controllers
                                            .Where(x => x.callsign == ConvertedCallsign)
                                            .ToString();
                                        break;
                                    case "atis":
                                        Result = Data.atis
                                            .Where(x => x.callsign == ConvertedCallsign)
                                            .ToString();

                                        break;
                                    case "prefiles":
                                        Result = Data.prefiles
                                            .Where(x => x.callsign == ConvertedCallsign)
                                            .ToString();

                                        break;
                                }

                                if (Result != null && Result != "[]")
                                {
                                    return Json(Result);
                                }
                                else
                                {
                                    VatsimError InnerError = new VatsimError()
                                    {
                                        Error = "Controller not found"
                                    };
                                    return Json(InnerError);
                                }
                            }
                            //Should work
                            else if (
                                Callsign.Length <= 4
                                && (ConvertedType == "pilots" || ConvertedType == "controllers")
                            )
                            {
                                string Result = null;

                                if (ConvertedType == "controllers")
                                {
                                    List<Json.Json.Controller> AllControllers =
                                        new List<Json.Json.Controller>();

                                    foreach (var Controller in Data.controllers)
                                    {
                                        //TODO remove tolower?
                                        if (
                                            Controller.callsign
                                                .ToLower()
                                                .StartsWith(Callsign.ToLower())
                                        )
                                        {
                                            Json.Json.Controller AddController = Controller;
                                            AllControllers.Add(AddController);
                                        }
                                    }

                                    if (AllControllers.Count > 0)
                                    {
                                        return Json(AllControllers);
                                        AllControllers.Clear();
                                    }
                                    else
                                    {
                                        VatsimError InnerError = new VatsimError
                                        {
                                            Error = "No Controllers found for this Airport"
                                        };

                                        return Json(InnerError);
                                    }
                                }
                                else
                                {
                                    string[] AllowedTrafficTypes = new string[]
                                    {
                                        "inbounds",
                                        "outbounds",
                                    };

                                    if (TrafficType != null)
                                    {
                                        string ConvertedTrafficType = TrafficType.ToLower();
                                        List<Pilot> AllPilots = new List<Pilot>();

                                        if (ConvertedTrafficType == "inbounds")
                                        {
                                            foreach (var Pilot in Data.pilots)
                                            {
                                                if (Pilot.flight_plan != null)
                                                {
                                                    if (
                                                        Pilot.flight_plan.arrival
                                                            .ToLower()
                                                            .StartsWith(Callsign.ToLower())
                                                    )
                                                    {
                                                        Pilot CurrentPilot = Pilot;
                                                        AllPilots.Add(CurrentPilot);
                                                    }
                                                }
                                            }

                                            if (AllPilots.Count > 0)
                                            {
                                                return Json(AllPilots);
                                                AllPilots.Clear();
                                            }
                                            else
                                            {
                                                VatsimError InnerError = new VatsimError
                                                {
                                                    Error = "No Inbounds found"
                                                };

                                                return Json(InnerError);
                                            }
                                        }
                                        else if (ConvertedTrafficType == "outbounds")
                                        {
                                            foreach (var Pilot in Data.pilots)
                                            {
                                                if (Pilot.flight_plan != null)
                                                {
                                                    if (
                                                        Pilot.flight_plan.departure
                                                            .ToLower()
                                                            .StartsWith(Callsign.ToLower())
                                                    )
                                                    {
                                                        Pilot CurrentPilot = Pilot;
                                                        AllPilots.Add(CurrentPilot);
                                                    }
                                                }
                                            }

                                            if (AllPilots.Count > 0)
                                            {
                                                return Json(AllPilots);
                                                AllPilots.Clear();
                                            }
                                            else
                                            {
                                                VatsimError InnerError = new VatsimError
                                                {
                                                    Error = "No Inbounds found"
                                                };

                                                return Json(InnerError);
                                            }
                                        }
                                        else
                                        {
                                            VatsimError InnerError = new VatsimError
                                            {
                                                Error =
                                                    "Traffic-Type not found, use \" inbounds \" or \" outbounds \" "
                                            };
                                            return Json(InnerError);
                                        }
                                    }
                                    else
                                    {
                                        List<Pilot> AllPilots = new List<Pilot>();
                                        foreach (var Pilot in Data.pilots)
                                        {
                                            if (Pilot.flight_plan != null)
                                            {
                                                if (
                                                    Pilot.flight_plan.departure
                                                        .ToLower()
                                                        .StartsWith(Callsign.ToLower())
                                                    || Pilot.flight_plan.arrival
                                                        .ToLower()
                                                        .StartsWith(Callsign.ToLower())
                                                )
                                                {
                                                    Pilot CurrentPilot = Pilot;

                                                    AllPilots.Add(CurrentPilot);
                                                }
                                            }
                                        }

                                        if (AllPilots != null)
                                        {
                                            return Json(AllPilots);
                                        }
                                    }
                                }

                            }
                            //else
                            //{
                            //    VatsimError InnerError = new VatsimError()
                            //    {
                            //        Error = "Controller not found"
                            //    };
                            //    return JsonConvert.SerializeObject(InnerError);
                            //}
                        }
                    }
                    VatsimError Error = new VatsimError() { Error = "Type not found" };
                    return Json(Error);
                }
            }
            else
            {
                VatsimError Error = new VatsimError() { Error = "Type not found" };
                return Json(Error);
            }
        }
    }
}
