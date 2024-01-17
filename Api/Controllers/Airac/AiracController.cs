namespace Api.Controllers.Airac
{
    [Route("api")]
    [ApiController]
    public class AiracController : Controller
    {
        /// <summary>
        /// Get all Airac Cycles within -1 and +3 years
        /// </summary>
        /// <remarks>
        /// uses https://github.com/Tim-Unger/AiracGen
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airacs")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get() => Airacs.Get();

        /// <summary>
        /// Get all Airac Cycles from 1985 to 2061
        /// </summary>
        /// <remarks>
        /// uses https://github.com/Tim-Unger/AiracGen
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airacs/all")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAll() => Airacs.GetAll();


        /// <summary>
        /// Get the current Airac
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airacs/current")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetCurrent() => CurrentAirac.Get();

        /// <summary>
        /// Get the next Airac
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airacs/next")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetNext() => NextAirac.Get();

        /// <summary>
        /// Get a specific Airac by Ident
        /// </summary>
        /// <remarks>
        /// The ident is the four digit code of the Airac (2601)
        /// </remarks>
        /// <param name="inputIdent"></param>
        /// <returns></returns>
        [HttpGet("/airacs/ident/{inputIdent}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByIdent(string inputIdent) => ByIdent.Get(inputIdent);

        /// <summary>
        /// Get all published Airacs in a specific year
        /// </summary>
        /// <param name="inputYear"></param>
        /// <returns></returns>
        [HttpGet("/airacs/year/{inputYear}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByYear(string inputYear) => ByYear.Get(inputYear);

        /// <summary>
        /// Get the Airac for a specific date
        /// </summary>
        /// <remarks>
        /// Provide the date in the ISO8601 format (2026_01_01)
        /// </remarks>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        [HttpGet("/airacs/date/{inputDate}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByDate(string inputDate) => ByDate.Get(inputDate);
    }
}
