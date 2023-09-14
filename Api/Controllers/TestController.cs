namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TestController : Controller
    {
        //[HttpGet("/fizzbuzz/{numberInput?}")]
        private string Test(string? numberInput = null)
        {
            if(numberInput == null)
            {
                var stringBuilder = new StringBuilder();

                Enumerable.Range(0, 100)
                     .ToList()
                     .ForEach
                        (x => stringBuilder.AppendLine(
                            x switch
                        {
                            int when x % 3 == 0 && x % 5 == 0 => "FizzBuzz",
                            int when x % 3 == 0 => "Fizz",
                            int when x % 5 == 0 => "Buzz",
                            _ => x.ToString()
                        }));

                return stringBuilder.ToString();
            }
            if(!int.TryParse(numberInput, out var number))
            {
                return "Input was not a number";
            }

            return number switch
            {
                int when number % 3 == 0 && number % 5 == 0 => "FizzBuzz",
                int when number % 3 == 0 => "Fizz",
                int when number % 5 == 0 => "Buzz",
                _ => number.ToString()
            };
        }

        //[HttpGet("/acars/{search}")]
        private string Get(string search)
        {
            var client = new HttpClient();
            var res = client.GetStringAsync("https://acars.adsbexchange.com/#!/1?socketid=t5nqLU8mhfIkql2yCmHm").Result;
            return "";
        }
    }
}
