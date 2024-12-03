using System.ComponentModel.DataAnnotations;
using WebApi.enums;
using WebApi.Validators;

namespace WebApi.Models
{
    public class User
    {
        public long Id { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }
        [Required]
        [MaxLength(255)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public required string LastName { get; set; }
        [Required]
        [ExactLengthLong(11)]
        public required long PersonalId { get; set; }
        public string? Avatar { get; set; }
        [Required]
        [MinLength(8)]
        public required string Password { get; set; }
        public required UserRole Role { get; set; }
        public string? ActivationCode { get; set; }
}
}
