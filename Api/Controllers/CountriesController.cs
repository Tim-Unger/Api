using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api")]
    public class CountriesController : Controller
    {
        [HttpGet("/countries")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string Get() => System.IO.File.ReadAllText(@".\countries.txt");
    }
}
