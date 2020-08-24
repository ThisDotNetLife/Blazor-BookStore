using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blazor_BookStore_API.Contracts;
using Blazor_BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Blazor_BookStore_API.Controllers {
    /// <summary>
    /// REST endpoint used for authentication and authorization.
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser>   _userInManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser>   userInManager,
                               ILoggerService logger,
                               IConfiguration config) {
            _signInManager = signInManager;
            _userInManager = userInManager;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// User Login Endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");
                if (userDTO == null) {
                    _logger.LogWarn($"{location}: Bad request; userDTO not provided.");
                    return BadRequest();
                } else {
                    var userName = userDTO.UserName;
                    var password = userDTO.Password;
                    var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);

                    if (result.Succeeded) {
                        _logger.LogInfo($"{location}: Successful authentication.");
                        var user = await _userInManager.FindByNameAsync(userName);
                        var tokenString = await GenerateJSONWebToken(user);
                        return Ok(new { token = tokenString });
                    } else {
                        _logger.LogWarn($"{location}: Unsuccessful authentication.");
                        return Unauthorized(userDTO);
                    }                    
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userInManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"],
                claims, null, expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerActionName() {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string location, Exception ex) {
            _logger.LogError($"{location}: {ex.Message} - {ex.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact customer support.");
        }

    } /* End of Controller */
}
