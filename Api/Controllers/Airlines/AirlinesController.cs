using Api.Controllers.Airlines;
using static Api.Controllers.Airlines.Parameter;

namespace Api.Controllers
{
    [Route("api")]
    public class AirlinesController : Controller
    {
        internal static readonly Regex _searchRegex = new Regex(
            @"(?>(name|iata|icao|callsign|country|active)=(\w*))*(?:(?>&|$))"
        );

        internal class Search
        {
            public string SearchString { get; set; }
            public SearchParameter SearchParameter { get; set; }
        }

        /// <summary>
        /// Get All Airlines
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airlines")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAll() => Json(AirlinesJson.ReadJson());

        /// <summary>
        /// Get the Data about an Airline-ICAO
        /// </summary>
        /// <remarks>
        /// You can use the following search parameters:
        /// name={Name}
        /// iata={Iata},
        /// icao={Icao},
        /// callsign={Callsign},
        /// country={Country},
        /// active={yes(true)/no(false)}
        /// You can combine searches with an &
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airlines/{search}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string search) => Searches.Get(search);

        [HttpGet("/airlines/{search}/matchall")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetMultiple(string search) => GetSearchesMatchAll.Get(search);
    }
}
