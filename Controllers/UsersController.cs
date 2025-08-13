using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymLinkPro.Data;
using GymLinkPro.Models;

namespace GymLinkPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>An enumerable list of users.</returns>
        /// <example>
        /// GET /api/Users
        /// Response: [ { "UserId": 1, "FirstName": "John", ... }, ... ]
        /// </example>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <example>
        /// GET /api/Users/5
        /// Response: { "UserId": 5, "FirstName": "Jane", ... }
        /// </example>
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The User object to create.</param>
        /// <returns>The created user.</returns>
        /// <example>
        /// POST /api/Users
        /// Body: { "FirstName": "John", "LastName": "Doe", "Email": "john.doe@example.com", "Role": "Member" }
        /// Response: Created { "UserId": 10, "FirstName": "John", ... }
        /// </example>
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="user">The updated User object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/Users/5
        /// Body: { "UserId": 5, "FirstName": "Jane", "LastName": "Doe", "Role": "Admin" }
        /// Response: No Content
        /// </example>
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            int currentUserId = GetCurrentUserId();
            bool isSelf = currentUserId == user.UserId;
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isSelf && !isAdmin)
            {
                return Forbid();
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/Users/5
        /// Response: No Content
        /// </example>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int currentUserId = GetCurrentUserId();
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isAdmin)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }
    }
}
