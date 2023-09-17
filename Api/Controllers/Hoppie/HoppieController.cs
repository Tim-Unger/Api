namespace Api.Controllers.Hoppie
{
    [Route("api")]
    [ApiController]
    public class HoppieController : Controller
    {
        [HttpGet("/hoppie/stations")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAllStations() => Json(AllClients.Get());

        [HttpGet("/hoppie/stations/{station}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetStation(string station = "") => Json(SingleStation.Get(station));

        [HttpGet("/hoppie/conversation/{stationOne}/{stationTwo?}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetConversation(string stationOne = "", string? stationTwo = null) => Conversation.Get(stationOne, stationTwo);

        //TODO
        //[Route("/hoppie/send")]
        //[HttpPost]
        private string SendMessage(string logon, string from, string to, string type, string message, Exception argumentNullException)
        {
            var values = new string[]{ logon, from, to, type };
            if (values.Any(string.IsNullOrEmpty))
            {
                throw argumentNullException;
            }

            type = type.ToLower();

            if(MessageType.Get(type) == RequestType.Unknown)
            {
                throw new InvalidDataException("Message Type was not valid");
            }

            var url = $"https://www.hoppie.nl/acars/system/connect.html?logon={logon}&from={from}&to={to}&type={type}&packet={message}";

            var client = new HttpClient();

            var result = client.GetAsync(url).Result;

            return "Success";
        }
    }
}
