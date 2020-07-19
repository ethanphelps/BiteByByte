using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BiteByByteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // route: api/auth/token
        [HttpPost("token")]
        public ActionResult GetToken()
        {

            // TODO:
            // security key ( get this from secrets or environment variable )
            string securityKey = "security_keyyyyyyyyyyyyyyyyyy";

            // symmetric security key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            // create credentials for signing token
            var signingCredentials = 
                new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            // add claims
            
            // create token
            var token = new JwtSecurityToken(
                issuer: "smesk.in",
                audience: "readers",
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials
            );

            // return string version of the token to client
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}