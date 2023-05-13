using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("/fizzbuzz/{numberInput?}")]
        public string Test(string? numberInput = null)
        {
            if(numberInput == null)
            {
                var stringBuilder = new StringBuilder();

                for(int i = 1; i <= 100; i++)
                {
                    var fizzBuzz = i switch
                    {
                        int when i % 3 == 0 && i % 5 == 0 => "FizzBuzz",
                        int when i % 3 == 0 => "Fizz",
                        int when i % 5 == 0 => "Buzz",
                        _ => i.ToString()
                    };

                    stringBuilder.AppendLine(fizzBuzz);
                }

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
    }
}
