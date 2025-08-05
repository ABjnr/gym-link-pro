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
    public class ProjectLinksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectLinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectLinks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectLink>>> GetProjectLinks()
        {
            return await _context.ProjectLinks.ToListAsync();
        }

        // GET: api/ProjectLinks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectLink>> GetProjectLink(int id)
        {
            var projectLink = await _context.ProjectLinks.FindAsync(id);

            if (projectLink == null)
            {
                return NotFound();
            }

            return projectLink;
        }

        // PUT: api/ProjectLinks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectLink(int id, ProjectLink projectLink)
        {
            if (id != projectLink.ProjectLinkId)
            {
                return BadRequest();
            }

            _context.Entry(projectLink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectLinkExists(id))
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

        // POST: api/ProjectLinks
        [HttpPost]
        public async Task<ActionResult<ProjectLink>> PostProjectLink(ProjectLink projectLink)
        {
            projectLink.AddedByUserId = GetCurrentUserId();

            _context.ProjectLinks.Add(projectLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectLink), new { id = projectLink.ProjectLinkId }, projectLink);
        }

        // DELETE: api/ProjectLinks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectLink(int id)
        {
            var projectLink = await _context.ProjectLinks.FindAsync(id);
            if (projectLink == null)
            {
                return NotFound();
            }

            int currentUserId = GetCurrentUserId();

            bool isOwner = projectLink.AddedByUserId == currentUserId;

            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectLink.ProjectId && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isOwner && !isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.ProjectLinks.Remove(projectLink);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectLinkExists(int id)
        {
            return _context.ProjectLinks.Any(e => e.ProjectLinkId == id);
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
