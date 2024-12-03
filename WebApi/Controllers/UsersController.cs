using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Auth;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Packages;
using WebApi.Services;

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
        public IActionResult Login(LoginRequest loginRequest)
        {
            try
            {
                User? existingUser = pKG_USERS.GetUserByEmail(loginRequest.Email);
                if (existingUser == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var verificationResult = passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, loginRequest.Password);

                if (verificationResult != PasswordVerificationResult.Success)
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
                if (pKG_USERS.CheckUserByEmail(user.Email))
                {
                    return Conflict(new ErrorResponse{ ErrorCode = "EMAIL_TAKEN", ErrorMessage = "მომხმარებელი ამ ელ. ფოსტით უკვე არსებობს." });
                }

                if (pKG_USERS.GetUserByPersonalId(user.PersonalId))
                {
                    return Conflict(new ErrorResponse { ErrorCode = "ID_TAKEN", ErrorMessage = "მომხმარებელი ამ პირადი ნომრით უკვე არსებობს." });
                }

                if (!pKG_USERS.IsVerificationCodeValid(user.Email, user.ActivationCode)) {
                    return Conflict(new ErrorResponse { ErrorCode = "INVALID_CODE", ErrorMessage = "კოდი არაა ვალუდური." });
                }

                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

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

                bool existingUser = pKG_USERS.CheckUserByEmail(email);
                return Ok(new { emailExists = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("check-personal-id")]
        public IActionResult CheckPersonalId([FromQuery] long personalId)
        {
            try
            {
                if (personalId == 0)
                {
                    return BadRequest("Personal ID is required.");
                }

                bool existingUser = pKG_USERS.GetUserByPersonalId(personalId);
                return Ok(new { personalIdExists = existingUser });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] EmailVerificationRequest request)
        {
            try
            {

                await pKG_USERS.SendVerificationCode(request.Email);
                return Ok(new { message = "Verification code sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
