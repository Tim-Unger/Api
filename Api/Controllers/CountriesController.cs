namespace Api.Controllers
{
    [Route("api")]
    public class CountriesController : Controller
    {
        /// <summary>
        /// Get the English names of all countries
        /// </summary>
        /// <returns></returns>
        [HttpGet("/countries")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string Get() => System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/Data/countries.txt");
    }
}