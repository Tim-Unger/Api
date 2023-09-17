namespace Api.Controllers.Airac
{
    [Route("api")]
    [ApiController]
    public class AiracController : Controller
    {
        [HttpGet("/airacs")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get() => Airacs.Get();

        [HttpGet("/airacs/current")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetCurrent() => Current.Get();

        [HttpGet("/airacs/next")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetNext() => Next.Get();

        [HttpGet("/airacs/ident/{ident}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByIdent(string inputIdent) => ByIdent.Get(inputIdent);

        [HttpGet("/airacs/year/{inputYear}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByYear(string inputYear) => ByYear.Get(inputYear);

        [HttpGet("/airacs/date/{date}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByDate(string inputDate) => ByDate.Get(inputDate);
    }
}
