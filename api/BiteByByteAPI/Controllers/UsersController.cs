using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using BiteByByteAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BiteByByteAPI.Services;
using BiteByByteAPI.Entities;
using BiteByByteAPI.Models.Users;
using WebApi.Helpers;

namespace BiteByByteAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        
        // Get JWT auth token
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]UpdateModel model)
        {
            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                // update user 
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _userService.Delete(id);
            return Ok();
        }
        
        //
        // // old stuff
        // // GET: api/users
        // [HttpGet]
        // public ActionResult<IEnumerable<User>> Get() => _userService.GetAll();
        //
        // [HttpGet("{id:length(24)}", Name = "GetUser")]
        // public ActionResult<User> Get(string id)
        // {
        //     var user = _userService.GetById(id);
        //
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return user;
        // }
        //
        // [HttpPost]
        // public ActionResult<User> Create(User user, string password)
        // {
        //     _userService.Create(user, password);
        //
        //     return CreatedAtRoute("GetUser", new {id = user.Id.ToString()}, user);
        // }
        //
        // [HttpPut("{id:length(24)}")]
        // public IActionResult Update(string id, User userIn)
        // {
        //     var user = _userService.GetById(id);
        //
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _userService.Update(id, userIn);
        //
        //     return NoContent();
        // }
        //
        // [HttpDelete("{id:length(24)}")]
        // public IActionResult Delete(string id)
        // {
        //     var user = _userService.GetById(id);
        //
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _userService.Delete(user.Id);
        //
        //     return NoContent();
        // }
    }
}