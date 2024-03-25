using Api.Controllers.Airlines;

namespace Api.Controllers
{
    [Route("api")]
    public class AirlinesController : Controller
    {
        internal static readonly Regex _searchRegex =
            new(@"(?>(name|iata|icao|callsign|country|active)=(\w*))*(?:(?>&|$))");

        internal class Search
        {
            public string SearchString { get; set; }
            public SearchParameter SearchParameter { get; set; }
        }

        /// <summary>
        /// Get All Airlines
        /// </summary>
        /// <remarks>
        /// source: https://raw.githubusercontent.com/npow/airline-codes/master/airlines.json
        /// I cannot guarantee the correctness and up-to-dateness of the data
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines")]
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
                    RequestName = "Airlines"
                }
            );

            return Json(AirlinesJson.ReadJson(), Options.JsonOptions);
        }

        /// <summary>
        /// Get Airlines that match the defined Search Parameters
        /// </summary>
        /// <remarks>
        /// You can use the following search parameters:
        /// name={Name}
        /// iata={Iata},
        /// icao={Icao},
        /// callsign={Callsign},
        /// country={Country},
        /// You can combine searches with an &amp;
        ///
        /// This will return a separate JSON Search Result for each Parameter, if you want to only get one JSON-Class, please use MatchAny
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines/{search}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string search)
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airlines with Search",
                    Params = search
                }
            );

            return Searches.Get(search);
        }

        /// <summary>
        /// Get Airlines that Match All defined Search Parameters
        /// </summary>
        /// <remarks>
        /// You can use the following search parameters:
        /// name={Name}
        /// iata={Iata},
        /// icao={Icao},
        /// callsign={Callsign},
        /// country={Country},
        /// You can combine searches with an &amp;
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines/{search}/matchall")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetMultipleMatchAll(string search)
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airlines with search, match all",
                    Params = search
                }
            );

            return GetSearchesMultipleParameters.Get(search, true);
        }

        /// <summary>
        /// Get Airlines that Match Any defined Search Parameter
        /// </summary>
        /// <remarks>
        /// You can use the following search parameters:
        /// name={Name}
        /// iata={Iata},
        /// icao={Icao},
        /// callsign={Callsign},
        /// country={Country},
        /// You can combine searches with an &amp;
        /// </remarks>
        /// <example>
        /// callsign=lufthansa&amp;icao=dlh
        /// </example>
        /// <returns></returns>
        [HttpGet("/airlines/{search}/matchany")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetMultipleMatchAny(string search) 
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    IPAddress = HttpContext.Current.Request.UserHostAddress,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airlines with search, match any",
                    Params = search
                }
            );

            return GetSearchesMultipleParameters.Get(search, false);
        }
    }
}
