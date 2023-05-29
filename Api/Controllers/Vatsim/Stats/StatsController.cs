namespace Api.Controllers.Vatsim.Stats
{
    [Route("api")]
    [ApiController]
    public class StatsController : Controller
    {
        [HttpGet("/vatsim/stats/{cid}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get(string cid)
        {
            var isCid = long.TryParse(cid, out var cidParsed);

            if (!isCid || !IsCidCorrect(cidParsed))
            {
                return Json("Input was not a valid CID");
            }

            var client = new HttpClient();
            var stats = await client.GetFromJsonAsync<VatsimStats>($"https://api.vatsim.net/api/ratings/{cidParsed}/");

            return Json(stats);
        }
        
        private static bool IsCidCorrect(long input)
        {
            var inputString = input.ToString();

            //The length is a correct CID (between 6 and 7 letters)
            if (inputString.Length >= 6 || inputString.Length <= 7)
            {
                return false;
            }

            if (inputString.Length == 6)
            {
                //Checks whether the input, a shorter (older) CIDs starts with 8 or 9
                if (!new[] { 8, 9 }.Any(x => inputString[0] == x))
                {
                    return false;
                }

                return true;
            }

            //Otherwise the Length is correct and the first letter is disregardable, so length correct
            return true;
        }
    }
}