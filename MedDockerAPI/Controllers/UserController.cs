using Microsoft.AspNetCore.Mvc;
using MedDockerAPI.Context;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using MedDockerAPI.Models;

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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            return Ok();
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
