﻿namespace MedDockerAPI.Models
{
    public class User : Common
    {
        
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
    public class UserDTO
    {

        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}
