using Api.Controllers.Trains.VVS;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class VVSController : Controller
    {
        private static readonly CultureInfo _germany = new("de-DE");

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, Converters = { new DateTimeConverter("dd.MM HH:mm") } };

        [HttpGet("/vvs/departures/{station}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetDepartures(string station)
            => Json(VVSData.Get(station), _jsonOptions);

        [HttpGet("/vvs/departures/{station}/{transportType}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetDepartures(string station, string transportType)
        {
            //var lines = new string[]{ "s1", "s2", "s3", "s4", "s5", "s6", "s60" };

            VVSTrainType trainType = transportType.ToLower(_germany) switch
            {
                "s-bahn" or "sbahn" or
                "s" or "sb"
                //string when lines.Any(x => x == transportType.ToLower(_germany)) 
                    => VVSTrainType.SBahn,

                "u-bahn" or "ubahn" or
                "u" or "ub"
                    => VVSTrainType.UBahn,

                "bus" or
                "b"
                => VVSTrainType.Bus,

                "regionalbahn" or "regio" or "regional-bahn" or
                "rb" or "re"
                => VVSTrainType.Regionalbahn,

                _ => throw new ArgumentOutOfRangeException()

            };

            var data = VVSData.Get(station);

            var filteredData = new VVSTimetable() 
            {   RequestTime = data.RequestTime, 
                Station = data.Station, 
                Departures = data.Departures!.Where(x => x.TrainType == trainType).ToList() 
            };

           return Json(filteredData, _jsonOptions);
        }
    }
}
