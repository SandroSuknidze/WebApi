using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class EmailVerificationRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
