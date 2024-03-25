namespace Api.Controllers.Airports
{
    public class AirportsController : Controller
    {
        private static readonly List<char> _regionCodes =
            new()
            {
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'Y',
                'Z'
            };

        /// <summary>
        /// Get all Airports
        /// </summary>
        /// <remarks>
        /// Source: https://ourairports.com/data/
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAll()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airports"
                }
            );

            return Json(Airports.Read(), Options.JsonOptions);
        }

        //TODO SearchParameters
        /// <summary>
        /// Get the Airport that matches a specific ICAO-Code
        /// </summary>
        /// <param name="icao"></param>
        /// <returns></returns>
        [HttpGet("/airports/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string icao)
        {
            if (icao.Length != 4)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        IPAddress = HttpContext.Current.Request.UserHostAddress,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airport by ICAO",
                        Params = icao
                    }
                );

                return Json(
                    new ApiError("Please provide a four letter ICAO Code"),
                    Options.JsonOptions
                );
            }

            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airport by ICAO",
                    Params = icao
                }
            );

            return Json(
                Airports.Read().FirstOrDefault(x => x.Icao == icao.ToUpper()),
                Options.JsonOptions
            );
        }

        /// <summary>
        /// Get all Airports that are considered a commercial airport
        /// </summary>
        /// <remarks>
        /// this only shows airports that have a four letter ICAO-Code (so no tiny grass strips with letters and numbers as their ICAO) and an IATA-Code (IATA-Codes are only assigned to commercial airports)
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airports/commercialairports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetIcaoAirports()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Commercial Airports",
                }
            );

            return Json(
                Airports
                    .Read()
                    .Where(
                        x =>
                            _regionCodes.Any(y => x.Icao.StartsWith(y))
                            && x.Icao.Length == 4
                            && x.Icao.All(char.IsLetter)
                            && x.Iata is not null
                    ),
                Options.JsonOptions
            );
        }

        /// <summary>
        /// Get the count of all airports
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airports/count")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public int GetAirportCount()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airport count",
                }
            );

            return Airports.Read().Count;
        }
    }
}
