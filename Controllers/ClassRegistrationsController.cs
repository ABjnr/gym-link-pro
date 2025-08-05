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

        // GET: api/ClassRegistrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassRegistration>>> GetClassRegistrations()
        {
            return await _context.ClassRegistrations.ToListAsync();
        }

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

        // POST: api/ClassRegistrations
        [HttpPost]
        public async Task<ActionResult<ClassRegistration>> PostClassRegistration(ClassRegistration classRegistration)
        {
            classRegistration.MemberId = GetCurrentUserId();
            _context.ClassRegistrations.Add(classRegistration);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassRegistration), new { id = classRegistration.ClassRegistrationId }, classRegistration);
        }

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
