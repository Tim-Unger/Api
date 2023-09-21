namespace Api.Errors
{
    public class ApiError
    {
        public string Error { get; set; } = "";

        public ApiError(string message)
        {
            Error = message;
        }
    }

}
