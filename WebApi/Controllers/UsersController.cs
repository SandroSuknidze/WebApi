using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Auth;
using WebApi.Models;
using WebApi.Packages;
using WebApi.Services;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IJwtManager jwtManager, IPKG_USERS pKG_USERS, PasswordHasher<User> passwordHasher, IEmailService emailService) : ControllerBase
    {
        private readonly IJwtManager jwtManager = jwtManager;
        private readonly IPKG_USERS pKG_USERS = pKG_USERS;
        private readonly PasswordHasher<User> passwordHasher = passwordHasher;
        private readonly IEmailService emailService = emailService;

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            try
            {
                //var existingUser = pKG_USERS.GetUserByEmail(user.Email);
                //if (existingUser == null)
                //{
                //    return Unauthorized("Invalid email or password.");
                //}

                //if (existingUser.Password != user.Password)
                //{
                //    return Unauthorized("Invalid email or password.");
                //}

                //var token = jwtManager.GetToken(existingUser);
                //return Ok(token);
                return StatusCode(200);
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
                bool existingUser = pKG_USERS.GetUserByEmail(user.Email);
                if (existingUser)
                {
                    return Conflict("User with this email already exists.");
                }

                bool existingUser2 = pKG_USERS.GetUserByPersonalId(user.PersonalId);
                if (existingUser2)
                {
                    return Conflict("User with this personal ID already exists.");
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

        [HttpGet("check-email")]
        public IActionResult CheckEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }

                bool existingUser = pKG_USERS.GetUserByEmail(email);
                return Ok(new { emailExists = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }

                int verificationCode = CodeGenerator.GenerateVerificationCode();
                await emailService.SendVerificationEmailAsync(email, verificationCode);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
