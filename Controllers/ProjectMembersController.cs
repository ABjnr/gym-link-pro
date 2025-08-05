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
    public class ProjectMembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectMember>>> GetProjectMembers()
        {
            return await _context.ProjectMembers.ToListAsync();
        }

        // GET: api/ProjectMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectMember>> GetProjectMember(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }

        // POST: api/ProjectMembers
        [HttpPost]
        public async Task<ActionResult<ProjectMember>> PostProjectMember(ProjectMember projectMember)
        {
            int currentUserId = GetCurrentUserId();
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectMember.ProjectId && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectMember), new { id = projectMember.ProjectMemberId }, projectMember);
        }

        // PUT: api/ProjectMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectMember(int id, ProjectMember projectMember)
        {
            if (id != projectMember.ProjectMemberId)
            {
                return BadRequest();
            }

            int currentUserId = GetCurrentUserId();
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectMember.ProjectId && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.Entry(projectMember).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ProjectMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectMember(int id)
        {
            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember == null)
            {
                return NotFound();
            }

            int currentUserId = GetCurrentUserId();
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectMember.ProjectId && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.ProjectMembers.Remove(projectMember);
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
