using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class UsersController(JwtOptions jwtOptions) : ControllerBase
    {
        

        [HttpPost]
        [Route("login")]
        public ActionResult<string> AuthenticatUser(AuthenticationRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var TokenDecriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audiencs,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,request.UserName),
                    new (ClaimTypes.Email,"Farag@gmail.com")

                })
            };
            var securityToken = tokenHandler.CreateToken(TokenDecriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return Ok(accessToken);
        }
    }
}
