using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetailBanking.Models;
using RetailBanking.Repository;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RetailBanking.Controllers
{
    [Route("RetailBanking-api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly RetailBankingDbContext _context;
        public LoginController(IConfiguration configuration,RetailBankingDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        //[AllowAnonymous]
        //[HttpPost("Login")]
        //public async Task<IActionResult> Login([FromBody] Login request)
        //{
        //    var client = new HttpClient();
        //    var content = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("grant_type",_configuration["Login:grant_type"]!),
        //            new KeyValuePair<string, string>("client_id",_configuration["Login:client_id"]!),
        //            new KeyValuePair<string, string>("client_secret",_configuration["Login:client_secret"]!),
        //            new KeyValuePair<string, string>("scope",_configuration["Login:scope"]!),
        //            new KeyValuePair<string, string>("username",request.Username),
        //            new KeyValuePair<string, string>("password",request.Password)
        //        });

        //    var response =
        //        await client.PostAsync(
        //            "https://login.microsoftonline.com/d463edec-5f3e-4fc1-9d60-45c87054ae76/oauth2/v2.0/token",
        //            content);
        //    Console.WriteLine($"Status Code: {response.StatusCode}");

        //    var result = await response.Content.ReadAsStringAsync();

        //    Console.WriteLine(result);

        //    return Content(result, "application/json");
        //}

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestData request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(request.AccessToken);

            var email = jwt.Claims
                .FirstOrDefault(x => x.Type == "unique_name")
                ?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var role = await _context.UserRoles.Where(x => x.Email == email).FirstOrDefaultAsync();

            var appToken = GenerateJwt(email, role!.RoleName);

            return Ok(new
            {
                access_token = appToken,
                role = role
            });
        }
        [NonAction]
        private string GenerateJwt(string email, string role)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
