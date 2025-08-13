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
    public class ClassRegistrationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassRegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all class registrations.
        /// </summary>
        /// <returns>An enumerable list of class registrations.</returns>
        /// <example>
        /// GET /api/ClassRegistrations
        /// Response: [ { "ClassRegistrationId": 1, "MemberId": 2, ... }, ... ]
        /// </example>
        // GET: api/ClassRegistrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassRegistration>>> GetClassRegistrations()
        {
            return await _context.ClassRegistrations.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific class registration by ID.
        /// </summary>
        /// <param name="id">The ID of the class registration.</param>
        /// <returns>The class registration with the specified ID.</returns>
        /// <example>
        /// GET /api/ClassRegistrations/5
        /// Response: { "ClassRegistrationId": 5, "MemberId": 2, ... }
        /// </example>
        // GET: api/ClassRegistrations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassRegistration>> GetClassRegistration(int id)
        {
            var classRegistration = await _context.ClassRegistrations.FindAsync(id);
            if (classRegistration == null)
            {
                return NotFound();
            }
            return classRegistration;
        }

        /// <summary>
        /// Creates a new class registration.
        /// </summary>
        /// <param name="classRegistration">The ClassRegistration object to create.</param>
        /// <returns>The created class registration.</returns>
        /// <example>
        /// POST /api/ClassRegistrations
        /// Body: { "ClassId": 5, "Status": "Registered", "RegistrationDate": "2023-04-01T00:00:00" }
        /// Response: Created { "ClassRegistrationId": 10, "MemberId": 2, ... }
        /// </example>
        // POST: api/ClassRegistrations
        [HttpPost]
        public async Task<ActionResult<ClassRegistration>> PostClassRegistration(ClassRegistration classRegistration)
        {
            classRegistration.MemberId = GetCurrentUserId();
            _context.ClassRegistrations.Add(classRegistration);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassRegistration), new { id = classRegistration.ClassRegistrationId }, classRegistration);
        }

        /// <summary>
        /// Updates an existing class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration to update.</param>
        /// <param name="classRegistration">The updated ClassRegistration object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/ClassRegistrations/5
        /// Body: { "ClassRegistrationId": 5, "MemberId": 2, ... }
        /// Response: No Content
        /// </example>
        // PUT: api/ClassRegistrations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassRegistration(int id, ClassRegistration classRegistration)
        {
            if (id != classRegistration.ClassRegistrationId)
            {
                return BadRequest();
            }

            int currentUserId = GetCurrentUserId();
            bool isOwner = classRegistration.MemberId == currentUserId;
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid();
            }

            _context.Entry(classRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassRegistrationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/ClassRegistrations/5
        /// Response: No Content
        /// </example>
        // DELETE: api/ClassRegistrations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassRegistration(int id)
        {
            var classRegistration = await _context.ClassRegistrations.FindAsync(id);
            if (classRegistration == null)
            {
                return NotFound();
            }

            int currentUserId = GetCurrentUserId();
            bool isOwner = classRegistration.MemberId == currentUserId;
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isOwner && !isAdmin)
            {
                return Forbid();
            }

            _context.ClassRegistrations.Remove(classRegistration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClassRegistrationExists(int id)
        {
            return _context.ClassRegistrations.Any(e => e.ClassRegistrationId == id);
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
