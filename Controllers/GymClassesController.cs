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
    public class GymClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GymClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all gym classes.
        /// </summary>
        /// <returns>An enumerable list of gym classes.</returns>
        /// <example>
        /// GET /api/GymClasses
        /// Response: [ { "GymClassId": 1, "Name": "Yoga", ... }, ... ]
        /// </example>
        // GET: api/GymClasses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GymClass>>> GetGymClasses()
        {
            return await _context.GymClasses.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific gym class by ID.
        /// </summary>
        /// <param name="id">The ID of the gym class.</param>
        /// <returns>The gym class with the specified ID.</returns>
        /// <example>
        /// GET /api/GymClasses/5
        /// Response: { "GymClassId": 5, "Name": "Pilates", ... }
        /// </example>
        // GET: api/GymClasses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GymClass>> GetGymClass(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return gymClass;
        }

        /// <summary>
        /// Creates a new gym class.
        /// </summary>
        /// <param name="gymClass">The GymClass object to create.</param>
        /// <returns>The created gym class.</returns>
        /// <example>
        /// POST /api/GymClasses
        /// Body: { "Name": "Yoga", "StartTime": "09:00", "EndTime": "10:00", "TrainerId": 2, ... }
        /// Response: Created { "GymClassId": 10, "Name": "Yoga", ... }
        /// </example>
        // POST: api/GymClasses
        [HttpPost]
        public async Task<ActionResult<GymClass>> PostGymClass(GymClass gymClass)
        {
            gymClass.TrainerId = GetCurrentUserId();
            _context.GymClasses.Add(gymClass);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGymClass), new { id = gymClass.GymClassId }, gymClass);
        }

        /// <summary>
        /// Updates an existing gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to update.</param>
        /// <param name="gymClass">The updated GymClass object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/GymClasses/5
        /// Body: { "GymClassId": 5, "Name": "Pilates", "TrainerId": 2, ... }
        /// Response: No Content
        /// </example>
        // PUT: api/GymClasses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGymClass(int id, GymClass gymClass)
        {
            if (id != gymClass.GymClassId)
            {
                return BadRequest();
            }

            int currentUserId = GetCurrentUserId();
            bool isTrainer = gymClass.TrainerId == currentUserId;
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isTrainer && !isAdmin)
            {
                return Forbid();
            }

            _context.Entry(gymClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GymClassExists(id))
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
        /// Deletes a specific gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/GymClasses/5
        /// Response: No Content
        /// </example>
        // DELETE: api/GymClasses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGymClass(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }

            int currentUserId = GetCurrentUserId();
            bool isTrainer = gymClass.TrainerId == currentUserId;
            bool isAdmin = _context.Users.Any(u => u.UserId == currentUserId && u.Role == "Admin");

            if (!isTrainer && !isAdmin)
            {
                return Forbid();
            }

            _context.GymClasses.Remove(gymClass);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.GymClassId == id);
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
