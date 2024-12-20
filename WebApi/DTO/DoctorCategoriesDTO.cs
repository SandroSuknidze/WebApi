using System.ComponentModel.DataAnnotations;
using WebApi.enums;
using WebApi.Validators;

namespace WebApi.DTO
{
    public class DoctorCategoriesDTO
    {
        public long Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public required string Avatar { get; set; }
        public required Decimal Rating { get; set; }
    }
}
