using Microsoft.AspNetCore.Mvc;
using Api.Controllers.Vatsim.Events;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Api.Controllers.Vatsim.Events
{
    [Route("api")]
    [ApiController]
    internal partial class EventController : Controller
    {
        /// <summary>
        /// Get all current and future Vatsim-Events
        /// </summary>
        /// <returns></returns>
        [HttpGet("/vatsim/events")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> Get()
        {
            var client = new HttpClient();

            //Sometimes Cloudflare does weird stuff and adds a captcha
            try
            {
                return Json(await client.GetStringAsync("https://my.vatsim.net/api/v1/events/all"));
            }
            catch
            {
                return Json("Could not get Event-Data");
            }
        }

        /// <summary>
        /// Get Events that match a specific condition.
        /// Allowed Conditions are:
        /// "airport=": Airport or Country/Region (e.g. EGLL, EG, or E),
        /// "date=": Start-Date of the Event, you can use "today", "tomorrow", or a ISO8601 compliant date (2023-12-31),
        /// "name=": Name of the Event, uses Contains() to find the string you are looking for
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpGet("/vatsim/events/{condition}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<JsonResult> GetWithDate(string condition)
        {
            if(condition == null)
            { 
                return Json("Please provide a condition");
            }

            var client = new HttpClient();

            var events = new VatsimEvent.Event();

            try
            {
                events = await client
                    .GetFromJsonAsync<VatsimEvent.Event>("https://my.vatsim.net/api/v1/events/all");

                if(events == null)
                {
                    return Json("Events could not be loaded");
                }

                var filteredEvents = new List<VatsimEvent.Data>();
                switch (condition.ToLower())
                {
                    case string when condition.StartsWith("airport="):
                        var airport = Regex.Match(condition, "airport=(.*)")
                                        .Groups[1].Value
                                        .ToUpper();
                        filteredEvents = events.data.Where(x => x.airports.Any(x => x.icao == airport)).ToList();
                        break;

                    case string when condition.StartsWith("date="):
                        DateTime date = GetDate(Regex.Match(condition, "date=(.*)").Groups[1].Value);
                        filteredEvents = events.data.Where(x => x.start_time.Date == date.Date).ToList();
                        break;

                    case string when condition.StartsWith("name="):
                        var name = Regex.Match(condition, "name=(.*)")
                                        .Groups[1].Value;
                        filteredEvents = events.data.Where(x => x.name.Contains(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        break;

                    default:
                        return Json("the stated condition was not valid, please use airport=, date=, or name=");
                }

                return Json(filteredEvents);
            }
            catch
            {
                return Json("Could not get Event-Data");
            }
        }

        private static DateTime GetDate(string date)
        {
            if(date.ToLower() == "today")
            {
                return DateTime.UtcNow;
            }

            if(date.ToLower() == "tomorrow")
            {
                return DateTime.UtcNow.AddDays(1);
            }

            GroupCollection dateGroups = DateRegex().Match(date).Groups;
            var year = int.Parse(dateGroups[1].Value);
            var month = int.Parse(dateGroups[2].Value);
            var day = int.Parse(dateGroups[3].Value);

            return new DateTime(year, month, day);
        }

        //Regex to match if a date is correct and ISO8601 compliant
        [GeneratedRegex("20[0-9][0-9]-(1[0-2]|0[0-9])-(3[0-1]|[0-2][0-9])")]
        private static partial Regex DateRegex();
    }
}
