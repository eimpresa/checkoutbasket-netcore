using CheckoutBasket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Contracts;

namespace WebApplication1.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthorizeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public AuthorizeController(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Post([FromForm]SigninContract signinContract)
        {
            //This method returns user id from username and password.
            var user = await userService.GetUserByCredentials(signinContract.username, signinContract.password);
            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken
            (
                issuer: configuration["TokenIssuer"],
                audience: configuration["TokenAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SigningKey"])),
                        SecurityAlgorithms.HmacSha256)
            );


            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
