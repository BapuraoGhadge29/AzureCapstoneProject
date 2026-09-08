using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RetailBanking.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {        
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var client = new HttpClient();

            var content =
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type","password"),
                    new KeyValuePair<string, string>("client_id","b562e509-f932-4ebe-860d-0683151ba848"),
                    new KeyValuePair<string, string>("client_secret","mBW8Q~2XzCh4Dw2q~nbokf8Yliw3l6pAN15fHbHA"),
                    new KeyValuePair<string, string>("scope","api://77696f1f-5f2f-4086-be7c-551cbea95991/.default"),
                    new KeyValuePair<string, string>("username",request.Username),
                    new KeyValuePair<string, string>("password",request.Password)
                });

            var response =
                await client.PostAsync(
                    "https://login.microsoftonline.com/d463edec-5f3e-4fc1-9d60-45c87054ae76/oauth2/v2.0/token",
                    content);

            var result =
                await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
    }
}
