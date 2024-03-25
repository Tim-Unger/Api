using AiracGen;
using Azure.Core;

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
        public JsonResult Get()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airacs"
                }
            );

            return Airacs.Get();
        }

        /// <summary>
        /// Get all Airac Cycles from 1985 to 2061
        /// </summary>
        /// <remarks>
        /// uses https://github.com/Tim-Unger/AiracGen
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/airacs/all")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetAll() 
        { 

            return Airacs.GetAll();
        }

        /// <summary>
        /// Get the current Airac
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airacs/current")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetCurrent()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Current Airac"
                }
            );

            return Airacs.GetCurrent();
        }

        /// <summary>
        /// Get the next Airac
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airacs/next")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetNext()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Next Airac"
                }
            );

            return Airacs.GetNext();
        }

        /// <summary>
        /// Get the previous Airac
        /// </summary>
        /// <returns></returns>
        [HttpGet("/airacs/previous")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetPrevious()
        {
            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Previous Airac"
                }
            );

            return Airacs.GetPrevious();
        }

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
        public JsonResult GetByIdent(string inputIdent)
        {
            //We can use discard as we only need to know if the input is not an int
            if (!int.TryParse(inputIdent, out _))
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airac by Ident"
                    }
                );

                return new JsonResult(
                    new ApiError("Provided Input was not a number"),
                    Options.JsonOptions
                );
            }

            if (inputIdent.Length != 4)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airac by Ident"
                    }
                );

                return new JsonResult(
                    new ApiError("Provided Input was not 4 letters long"),
                    Options.JsonOptions
                );
            }

            var airac = AiracGenerator.GenerateSingle(inputIdent);

            if (airac is null)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airac by Ident"
                    }
                );

                return new JsonResult(new ApiError("Ident not found"), Options.JsonOptions);
            }

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airac by Ident"
                }
            );

            return new JsonResult(airac, Options.JsonOptions);
        }

        /// <summary>
        /// Get all published Airacs in a specific year
        /// </summary>
        /// <param name="inputYear"></param>
        /// <returns></returns>
        [HttpGet("/airacs/year/{inputYear}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByYear(string inputYear)
        {
            if (!int.TryParse(inputYear, out _))
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airacs by Year"
                    }
                );

                return new JsonResult(
                    new ApiError("Provided input was not a number"),
                    Options.JsonOptions
                );
            }

            var dateRegex = new Regex(@"^(?>(20[2-3]\d)|([2-3]\d))$");

            var dateMatch = dateRegex.Match(inputYear.ToString());

            if (!dateMatch.Success)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airacs by Year"
                    }
                );

                return new JsonResult(
                    new ApiError("Year was not valid, please provide a valid year"),
                    Options.JsonOptions
                );
            }

            var yearRaw = dateMatch.Groups[1].Success
                ? dateMatch.Groups[1].Value
                : $"20{dateMatch.Groups[2].Value}";

            var isYear = int.TryParse(yearRaw, out var year);

            if (!isYear)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airacs by Year"
                    }
                );

                return new JsonResult(
                    new ApiError("Year was not valid, please provide a valid year"),
                    Options.JsonOptions
                );
            }

            //We can use == here as we do not have localization with numbers
            var airacs = AiracGenerator.GenerateByYear(year);

            if (airacs.Count == 0)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airacs by Year"
                    }
                );
                return new JsonResult(
                    new ApiError("No Airacs found for this year"),
                    Options.JsonOptions
                );
            }

            Logger.Log(
                new Logger.LogEntry()
                {
                    Request = Request,
                    RequestStatus = Logger.RequestStatus.Success,
                    ApiRequestType = "GET",
                    RequestName = "Airacs by Year"
                }
            );

            return new JsonResult(airacs, Options.JsonOptions);
        }

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
        public JsonResult GetByDate(string inputDate)
        {
            var airacs = Airacs.GetAiracList();

            var dateRegex = new Regex(
                @"(20[2-3][0-9])(?>_|-|)?(0[1-9]|1[0-2])(?>_|-|)?(0[1-9]|1[0-9]|2[0-9]|3[0-1])"
            );

            if (!dateRegex.IsMatch(inputDate))
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airac by Date"
                    }
                );

                return new JsonResult(
                    new ApiError(
                        "Inputted date was not valid, please use an ISO9601 compliant date (2023_12_31)"
                    ),
                    Options.JsonOptions
                );
            }

            var groups = dateRegex.Match(inputDate).Groups;

            var year = int.Parse(groups[1].Value);

            var month = int.Parse(groups[2].Value);

            var day = int.Parse(groups[3].Value);

            var dateOnly = new DateOnly(year, month, day);

            var correctAirac = airacs.FirstOrDefault(
                x => x.StartDate < dateOnly && x.EndDate > dateOnly
            );

            if (correctAirac is null)
            {
                Logger.Log(
                    new Logger.LogEntry()
                    {
                        Request = Request,
                        RequestStatus = Logger.RequestStatus.Error,
                        ApiRequestType = "GET",
                        RequestName = "Airac by Date"
                    }
                );

                return new JsonResult(new ApiError("Date has no Airac"), Options.JsonOptions);
            }

            return new JsonResult(correctAirac, Options.JsonOptions);
        }
    }
}
