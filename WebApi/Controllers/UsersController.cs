using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Auth;
using WebApi.Models;
using WebApi.Packages;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IJwtManager jwtManager, IPKG_USERS pKG_USERS, PasswordHasher<User> passwordHasher) : ControllerBase
    {
        private readonly IJwtManager jwtManager = jwtManager;
        private readonly IPKG_USERS pKG_USERS = pKG_USERS;
        private readonly PasswordHasher<User> passwordHasher = passwordHasher;

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            try
            {
                var existingUser = pKG_USERS.GetUserByEmail(user.Email);
                if (existingUser == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                if (existingUser.Password != user.Password)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = jwtManager.GetToken(existingUser);
                return Ok(token);
            }

            catch (KeyNotFoundException)
            {
                return Unauthorized("Invalid email or password.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal error occurred." + ex);
            }
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            try
            {
                var existingUser = pKG_USERS.GetUserByEmail(user.Email);
                if (existingUser != null)
                {
                    return Conflict("User with this email already exists.");
                }
                //user.Password = passwordHasher.HashPassword(user, user.Password);
                pKG_USERS.AddUser(user);
                var token = jwtManager.GetToken(user);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("validateToken")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new { valid = true });
        }

    }
}
