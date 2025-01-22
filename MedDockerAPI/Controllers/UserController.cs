using Microsoft.AspNetCore.Mvc;
using MedDockerAPI.Context;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using MedDockerAPI.Models;
using System.Text.RegularExpressions;


namespace MedDockerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public UserController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if(await _Context.Users.AnyAsync(u =>  u.Username == userDTO.Username))
            {
                return Conflict(new { message = "Username already in use" });
            }
            if(await _Context.Users.AnyAsync(u =>u.Email == userDTO.Email))
            {
                return Conflict(new {message = "Email is already in use"});
            }
            if (!IsPasswordSecure(userDTO.Password))
            {
                return Conflict(new {message = "Password is not secure"});
            }
            var user = MapCreateUserDTO(userDTO);
            _Context.Users.Add(user);

            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
            return Ok("User signed up successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllUsersById(string id)
        {
            var user = await _Context.Users.FindAsync(id);

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await _Context.Users.ToListAsync();

            return Ok(user);
        }

        [HttpGet("FromQuery")]
        public async Task<IActionResult> GetAllUsersFromQuery([FromQuery] string username, [FromQuery] string password)
        {
            var users = await _Context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.Password == password);




            return Ok(users);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var user = await _Context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Not found");
            }
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            await _Context.SaveChangesAsync();

            return Ok(user);
        }
        [HttpPut("Query")]
        public async Task<IActionResult> UpdateUserQuery(string id, [FromQuery] User updatedUser)
        {
            var user = await _Context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Not found");
            }
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            await _Context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchUpdateUser(string id, [FromQuery] UserDTO user)
        {
            var existingUser = await _Context.Users.FindAsync(id);


            if (!string.IsNullOrEmpty(user.Username))
            {
                existingUser.Username = user.Username;
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser.Email = user.Email;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }
            await _Context.SaveChangesAsync();

            return Ok(existingUser);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _Context.Users.FindAsync(id);
            _Context.Users.Remove(user);
            await _Context.SaveChangesAsync();
            return (Ok());
        }


        private bool IsPasswordSecure(string password)
        {
            var hasUppercase = new Regex(@"[A-Z]+");
            var hasLowercase = new Regex(@"[a-z]+");
            var hasNumbers = new Regex(@"[0-9]+");
            var hasSpecialChars = new Regex(@"[\W_]+");
            var hasMinimumChars = new Regex(@".{8,}");

            return hasUppercase.IsMatch(password) &&
                hasLowercase.IsMatch(password) &&
                hasNumbers.IsMatch(password) &&
                hasSpecialChars.IsMatch(password) &&
                hasMinimumChars.IsMatch(password);
        }

        private User MapCreateUserDTO(UserDTO userDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            string salt = hashedPassword.Substring(0, 29);

            return new User
            {
                ID = Guid.NewGuid().ToString("N"),
                Email = userDTO.Email,
                Username = userDTO.Username,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
                Password = userDTO.Password,
                HashedPassword = hashedPassword,
                Salt = salt,
            };
        }

    }
}
