namespace MedDockerAPI.Models
{
    public class User : Common
    {
        
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class UserDto
    {

        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}
