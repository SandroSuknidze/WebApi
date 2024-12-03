namespace WebApi.Models
{
    public class ErrorResponse
    {
        public required String ErrorCode { get; set; }
        public required String ErrorMessage { get; set; }
    }
}
