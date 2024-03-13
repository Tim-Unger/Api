namespace Api.Controllers.vACDMData
{
    [Route("api")]
    [ApiController]
    public class VACDMDataController : Controller
    {
        [HttpGet("/vacdm/datasources")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> GetDataSources() 
        {
            var client = new HttpClient();

            var dataSources = await client.GetFromJsonAsync<string[]>("https://raw.githubusercontent.com/Tim-Unger/Api/main/Api/Data/datasources.json");

            return Json(dataSources);
        }
        
    }
}