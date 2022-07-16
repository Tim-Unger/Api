using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers.Vatsim;
using Newtonsoft.Json;

namespace Api.Controllers
{
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
            return "";
        }

        [HttpGet("data")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetaAllData()
        {
            return Vatsim.GetData.GetVatsimData();
        }

        [HttpGet("data/{type}/{callsign?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetData(string Type = null, string Callsign = null)
        {
            string ConvertedType = Type.ToLower();

            if (AllowedTypes.Contains(ConvertedType))
            {
                string VatsimData = Vatsim.GetData.GetVatsimData();
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
                                    Result = JsonConvert.SerializeObject(Data.pilots.Where(x => x.cid == Cid));
                                    break;
                                case "controllers":
                                    Result = JsonConvert.SerializeObject(Data.controllers.Where(x => x.cid == Cid));
                                    break;
                                case "atis":
                                    Result = JsonConvert.SerializeObject(Data.atis.Where(x => x.cid == Cid));
                                    break;
                                case "prefiles":
                                    Result = JsonConvert.SerializeObject(Data.prefiles.Where(x => x.cid == Cid));
                                    break;
                            }

                            if (Result != null && Result != "[]")
                            {
                                return Result;
                            }

                            else
                            {
                                VatsimError InnerError = new VatsimError() { Error = "Controller not found" };
                                return JsonConvert.SerializeObject(InnerError);
                            }
                        }

                        //Allow FIRs, Airports, etc
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
                                        Result = JsonConvert.SerializeObject(Data.pilots.Where(x => x.callsign == ConvertedCallsign));
                                        break;
                                    case "controllers":
                                        Result = JsonConvert.SerializeObject(Data.controllers.Where(x => x.callsign == ConvertedCallsign));
                                        break;
                                    case "atis":
                                        Result = JsonConvert.SerializeObject(Data.atis.Where(x => x.callsign == ConvertedCallsign));
                                        break;
                                    case "prefiles":
                                        Result = JsonConvert.SerializeObject(Data.prefiles.Where(x => x.callsign == ConvertedCallsign));
                                        break;
                                }

                                if (Result != null && Result != "[]")
                                {
                                    return Result;
                                }

                                else
                                {
                                    VatsimError InnerError = new VatsimError() { Error = "Controller not found" };
                                    return JsonConvert.SerializeObject(InnerError);
                                }
                            }
                            else
                            {
                                VatsimError InnerError = new VatsimError() { Error = "Controller not found" };
                                return JsonConvert.SerializeObject(InnerError);
                            }
                        }
                    }
                    VatsimError Error = new VatsimError() { Error = "Type not found" };
                    return JsonConvert.SerializeObject(Error); ;
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
