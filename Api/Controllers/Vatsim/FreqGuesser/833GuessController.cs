using AviationSharp.Vatsim;
using System.Text.RegularExpressions;

namespace Api.Controllers.Vatsim.FreqGuesser
{
    //TODO
    public class FreqGuessController : Controller
    {
        //[HttpGet("/guess833/{frequency}")]
        public JsonResult Get(string frequency = "")
        {
            if (string.IsNullOrWhiteSpace(frequency))
            {
                throw new ArgumentException($"'{nameof(frequency)}' cannot be null or whitespace.", nameof(frequency));
            }

            var frequencyRegex = new Regex(@"(1[1-3]\d)(?>\.)?(\d{1,3})");

            var frequencyGroups = frequencyRegex.Match(frequency).Groups ?? throw new InvalidDataException("Frequency could not be read");

            var frequencyParsed = $"{frequencyGroups[1].Value}.{frequencyGroups[2].Value}";

            frequencyParsed = frequencyParsed.Length switch
            {
                5 => $"{frequencyParsed}00",
                6 => $"{frequencyParsed}0",
                7 => frequencyParsed,
                _ => throw new ArgumentOutOfRangeException()
            };

            var frequencies = VatsimData.GetAllControllers()
                .Select(x => x.Frequency)
                .ToList();

            return Json("");
        }
    }
}
