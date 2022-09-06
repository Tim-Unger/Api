﻿using Microsoft.AspNetCore.Http;
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
        public string GetStatus()
        {
            Vatsim.GetStatus.Status();

            return JsonConvert.SerializeObject(Data.Servers);
        }

        [HttpGet("data")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public string GetaAllData()
        {
            string VatsimData = null;
            try
            {
                VatsimData = Vatsim.GetData.GetVatsimData();
                return VatsimData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet("data/{type}/{callsign?}/{traffictype?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public string GetData(string Type = null, string Callsign = null, string TrafficType = null)
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
                    return ex.Message;
                }
                if (Callsign == null)
                {
                    if (!string.IsNullOrEmpty(VatsimData))
                    {
                        Rootobject Data = JsonConvert.DeserializeObject<Rootobject>(VatsimData);

                        switch (ConvertedType)
                        {
                            case "general":
                                return JsonConvert.SerializeObject(Data.general);
                            case "pilots":
                                return JsonConvert.SerializeObject(Data.pilots);
                            case "controllers":
                                return JsonConvert.SerializeObject(Data.controllers);
                            case "atis":
                                return JsonConvert.SerializeObject(Data.atis);
                            case "servers":
                                return JsonConvert.SerializeObject(Data.servers);
                            case "prefiles":
                                return JsonConvert.SerializeObject(Data.prefiles);
                            case "facilities":
                                return JsonConvert.SerializeObject(Data.facilities);
                            case "ratings":
                                return JsonConvert.SerializeObject(Data.ratings);
                            case "pilot_ratings":
                                return JsonConvert.SerializeObject(Data.pilot_ratings);
                        }
                    }
                    return "";
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
                                    Result = JsonConvert.SerializeObject(
                                        Data.pilots.Where(x => x.cid == Cid)
                                    );
                                    break;
                                case "controllers":
                                    Result = JsonConvert.SerializeObject(
                                        Data.controllers.Where(x => x.cid == Cid)
                                    );
                                    break;
                                case "atis":
                                    Result = JsonConvert.SerializeObject(
                                        Data.atis.Where(x => x.cid == Cid)
                                    );
                                    break;
                                case "prefiles":
                                    Result = JsonConvert.SerializeObject(
                                        Data.prefiles.Where(x => x.cid == Cid)
                                    );
                                    break;
                            }

                            if (Result != null && Result != "[]")
                            {
                                return Result;
                            }
                            else
                            {
                                VatsimError InnerError = new VatsimError()
                                {
                                    Error = "Controller not found"
                                };
                                return JsonConvert.SerializeObject(InnerError);
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
                                        Result = JsonConvert.SerializeObject(
                                            Data.pilots.Where(x => x.callsign == ConvertedCallsign)
                                        );
                                        break;
                                    case "controllers":
                                        Result = JsonConvert.SerializeObject(
                                            Data.controllers.Where(
                                                x => x.callsign == ConvertedCallsign
                                            )
                                        );
                                        break;
                                    case "atis":
                                        Result = JsonConvert.SerializeObject(
                                            Data.atis.Where(x => x.callsign == ConvertedCallsign)
                                        );
                                        break;
                                    case "prefiles":
                                        Result = JsonConvert.SerializeObject(
                                            Data.prefiles.Where(
                                                x => x.callsign == ConvertedCallsign
                                            )
                                        );
                                        break;
                                }

                                if (Result != null && Result != "[]")
                                {
                                    return Result;
                                }
                                else
                                {
                                    VatsimError InnerError = new VatsimError()
                                    {
                                        Error = "Controller not found"
                                    };
                                    return JsonConvert.SerializeObject(InnerError);
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
                                        return JsonConvert.SerializeObject(AllControllers);
                                        AllControllers.Clear();
                                    }
                                    else
                                    {
                                        VatsimError InnerError = new VatsimError
                                        {
                                            Error = "No Controllers found for this Airport"
                                        };

                                        return JsonConvert.SerializeObject(InnerError);
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
                                                Result = JsonConvert.SerializeObject(AllPilots);
                                                AllPilots.Clear();
                                            }
                                            else
                                            {
                                                VatsimError InnerError = new VatsimError
                                                {
                                                    Error = "No Inbounds found"
                                                };

                                                return JsonConvert.SerializeObject(InnerError);
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
                                                Result = JsonConvert.SerializeObject(AllPilots);
                                                AllPilots.Clear();
                                            }
                                            else
                                            {
                                                VatsimError InnerError = new VatsimError
                                                {
                                                    Error = "No Inbounds found"
                                                };

                                                return JsonConvert.SerializeObject(InnerError);
                                            }
                                        }
                                        else
                                        {
                                            VatsimError InnerError = new VatsimError
                                            {
                                                Error =
                                                    "Traffic-Type not found, use \" inbounds \" or \" outbounds \" "
                                            };
                                            return JsonConvert.SerializeObject(InnerError);
                                        }
                                        
                                    }

                                    else
                                    {
                                        List<Pilot> AllPilots = new List<Pilot>();
                                        foreach (var Pilot in Data.pilots)
                                        {
                                            if (Pilot.flight_plan != null)
                                            {
                                                if (Pilot.flight_plan.departure.ToLower().StartsWith(Callsign.ToLower()) || Pilot.flight_plan.arrival.ToLower().StartsWith(Callsign.ToLower()))
                                                {
                                                    Pilot CurrentPilot = Pilot;

                                                    AllPilots.Add(CurrentPilot);
                                                }
                                            }
                                        }

                                        Result = JsonConvert.SerializeObject(AllPilots);
                                    }
                                    
                                }

                                if (Result != null && Result != "[]")
                                {
                                    return Result;
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
                    return JsonConvert.SerializeObject(Error);
                    ;
                }
            }
            else
            {
                VatsimError Error = new VatsimError() { Error = "Type not found" };
                return JsonConvert.SerializeObject(Error);
            }
        }
    }
}
