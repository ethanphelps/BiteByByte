// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Net;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using BiteByByteAPI.Helpers;
// using BiteByByteAPI.Models;
// using BiteByByteAPI.Models.Identity;
// using Microsoft.AspNet.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.Tokens;
//
// namespace BiteByByteAPI.Controllers
// {
//     public class IdentityController : ApiController
//     {
//         private readonly UserManager<User> userManager;
//         private readonly AppSettings appSettings;
//
//         public IdentityController(UserManager<User> userManager,
//             IOptions<AppSettings> appSettings
//         )
//         {
//             this.userManager = userManager;
//             this.appSettings = appSettings.Value;
//         }
//         
//         [Route(nameof(Register))]
//         public async Task<ActionResult> Register(RegisterUserRequestModel model)
//         {
//             var user = new User
//             {
//                 Email = model.Email,
//                 UserName = model.Username
//             };
//
//             var result = await this.userManager.CreateAsync(user, model.Password);
//
//             if (result.Succeeded)
//             {
//                 return Ok();
//             }
//
//             return this.BadRequest(result.Errors);
//         }
//
//         [Route(nameof(Login))]
//         public async Task<ActionResult<string>> Login(LoginRequestModel model)
//         {
//             var user = await this.userManager.FindByNameAsync(model.Username);
//             if (user == null)
//             {
//                 return Unauthorized();
//             }
//
//             // validate where user can login or not
//             var passwordValid = await this.userManager.CheckPasswordAsync(user, model.Password);
//
//             if (!passwordValid)
//             {
//                 return Unauthorized();
//             }
//
//                 // generate token that is valid for 7 days
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(appSettings.Secret);
//             var tokenDescriptor = new SecurityTokenDescriptor
//             {
//                 Subject = new ClaimsIdentity(new Claim[] 
//                 {
//                     new Claim(ClaimTypes.Name, user.Id.ToString())
//                 }),
//                 Expires = DateTime.UtcNow.AddDays(7),
//                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//             };
//             var token = tokenHandler.CreateToken(tokenDescriptor);
//             var encryptedToken = tokenHandler.WriteToken(token);
//
//             return encryptedToken;
//         }
//     }
// }