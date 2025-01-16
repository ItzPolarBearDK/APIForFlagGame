using Microsoft.AspNetCore.Mvc;
using MedDockerAPI.Context;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using MedDockerAPI.Models;
using Microsoft.Extensions.Logging;

namespace MedDockerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _Context;
        private readonly ILogger<UserController> _logger;

        public UserController(DatabaseContext context, ILogger<UserController> logger)
        {
            _Context = context;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] User user)
        {
            _logger.LogInformation("Received CreateUser request: {@User}", user);

            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                // Map request to a User entity
                var users = new User
                {
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password // Ideally, hash the password here
                };

                // Add user to database
                _Context.Users.Add(user);

                // Commit changes to the database
                await _Context.SaveChangesAsync();

                _logger.LogInformation("User successfully created: {Username}", user.Username);

                return Ok(new { Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user");
                return StatusCode(500, new { Message = "An error occurred while creating the user." });
            }
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
        public async Task<IActionResult> PatchUpdateUser(string id, [FromQuery] UserDto user)
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

    }
}
