using System.Net;

namespace Api
{
    public class Logger
    {
        public enum RequestStatus
        {
            Success,
            Error,
            Fatal,
        }

        public class LogEntry
        {
            public IPAddress? IPAddress { get; set; }

            public RequestStatus RequestStatus { get; set; }

            public string ApiRequestType { get; set; }

            public string RequestName { get; set; }

            public string? Params { get; set; }
        }

        public static void Log(LogEntry entry) 
        {
            var now = DateTime.UtcNow;

            var ip = entry.IPAddress.ToString() ?? "Unknow Host";

            var status = entry.RequestStatus.ToString().ToUpper();

            var stringBuilder = new StringBuilder($"[{now:s}Z] [{status}] [{entry.ApiRequestType}] {ip} Request: {entry.RequestName}");

            var entryParams = entry.Params;

            if(entryParams is not null)
            {
                stringBuilder.Append($", Parameters: {entryParams}");
            }

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
