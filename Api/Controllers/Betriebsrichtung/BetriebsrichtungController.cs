﻿namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class BetriebsrichtungController : Controller
    {
        /// <summary>
        /// Get the current landing direction and forecast for the next days at EDDF as a JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Betriebsrichtung",
                }
            );

            return Json(GetBetriebsrichtung.Get(), Options.JsonOptions);
        }

        /// <summary>
        /// Get the current landing direction at EDDF as an int (RWY-direction only, nothing else)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung/raw")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetRaw()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Betriebsrichtung Raw",
                }
            );

            return GetBetriebsrichtung.Get().Richtung;
        }

        /// <summary>
        /// Get the current landing direction and the forecast for the next days decoded
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung/decode")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public string GetDecoded()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Betriebsrichtung decoded",
                }
            );

            return BetriebsrichtungDecoder.Decode(GetBetriebsrichtung.Get());
        }
    }
}
