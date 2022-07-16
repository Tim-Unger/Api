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
        public string GetaAllData()
        {
            return Vatsim.GetData.GetVatsimData();
        }

        //TODO JSON
        [HttpGet("data/{type?}")]
        public string GetData(string Type = null)
        {
            string ConvertedType = Type.ToLower();
            if (AllowedTypes.Contains(ConvertedType))
            {
                string VatsimData = Vatsim.GetData.GetVatsimData();
                if (!string.IsNullOrEmpty(VatsimData))
                {
                    Rootobject Data = JsonConvert.DeserializeObject<Rootobject>(VatsimData);

                    switch (ConvertedType)
                    {
                        case "general":
                            return JsonConvert.SerializeObject(Data.general);
                            break;
                        case "pilots":
                            return JsonConvert.SerializeObject(Data.pilots);
                            break;
                        case "controllers":
                            return JsonConvert.SerializeObject(Data.controllers);
                            break;
                        case "atis":
                            return JsonConvert.SerializeObject(Data.atis);
                            break;
                        case "servers":
                            return JsonConvert.SerializeObject(Data.servers);
                            break;
                        case "prefiles":
                            return JsonConvert.SerializeObject(Data.prefiles);
                            break;
                        case "facilities":
                            return JsonConvert.SerializeObject(Data.facilities);
                            break;
                        case "ratings":
                            return JsonConvert.SerializeObject(Data.ratings);
                            break;
                        case "pilot_ratings":
                            return JsonConvert.SerializeObject(Data.pilot_ratings);
                            break;

                    }
                }
                return "";
            }
            else
            {
                VatsimError Error = new VatsimError()
                {
                    Error = "Type not found"
                };
                return JsonConvert.SerializeObject(Error);
            }
        }
    }
    
}
