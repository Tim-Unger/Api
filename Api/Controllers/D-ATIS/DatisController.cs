using static Api.Controllers.DAtis.DAtisAirportsClass;

namespace Api.Controllers.DAtis
{
    [ApiController]
    [Route("api")]
    public class DatisController : Controller
    {
        /// <summary>
        /// Get all supported D-ATIS Airports
        /// </summary>
        /// <returns></returns>
        [HttpGet("/datis/airports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetAirports()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Connection.RemoteIpAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "D-ATIS Airports",
                }
            );

            return string.Join(Environment.NewLine, DAtisAirports);
        }

        /// <summary>
        /// Get the D-Atis for an American Airport
        /// </summary>
        /// <remarks>
        /// Please use /datis/airports to see a list of supported airports.
        /// Append /textonly to only get the Text of the D-ATIS
        /// </remarks>
        /// <param name="icao">the four or three letter ICAO of the Airport</param>
        /// <param name="textOnly">whether you would like the D-ATIS text only</param>
        /// <returns></returns>
        [HttpGet("/datis/{icao}/{textOnly?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get(string icao, string? textOnly = null)
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Connection.RemoteIpAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "D-ATIS for Airport",
                    Params = icao
                }
            );

            return Datis.Get(icao, textOnly);
        }
    }
}
