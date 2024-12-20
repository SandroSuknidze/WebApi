using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Packages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(IPKG_DOCTORS pKG_DOCTORS) : ControllerBase
    {
        private readonly IPKG_DOCTORS pKG_DOCTORS = pKG_DOCTORS;

        [HttpGet]
        public IEnumerable<DoctorCategoriesDTO> Get([FromQuery] string? name, [FromQuery] string? category)
        {
            var doctors = pKG_DOCTORS.GetDoctors(name, category);
            return doctors;
        }

        // GET api/<DoctorsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDoctor([FromForm]DoctorDTO doctorDto, IFormFile avatar, IFormFile bio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string avatarBase64 = null;

            if (avatar != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await avatar.CopyToAsync(memoryStream);
                    avatarBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            string bioBytes = null;
            if (bio != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    bio.CopyTo(memoryStream);
                    bioBytes = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            var passwordHasher = new PasswordHasher<DoctorDTO>();
            var hashedPassword = passwordHasher.HashPassword(doctorDto, doctorDto.Password);

            var doctor = new Doctor
            {
                FirstName = doctorDto.FirstName,
                LastName = doctorDto.LastName,
                Email = doctorDto.Email,
                PersonalId = doctorDto.PersonalId,
                Password = hashedPassword,
                CategoryId = doctorDto.CategoryId,
                Role = doctorDto.Role,
                Avatar = avatarBase64,
                Bio = bioBytes,
            };

            pKG_DOCTORS.AddDoctor(doctor);

            return Ok(new { message = "Doctor registered successfully!" });
        }

        // PUT api/<DoctorsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<DoctorsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
