namespace Animed.Application.Dtos
{
    public class GetAuthDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Jwt { get; set; } = string.Empty;
    }
}
