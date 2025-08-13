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

        /// <summary>
        /// Retrieves all project members.
        /// </summary>
        /// <returns>An enumerable list of project members.</returns>
        /// <example>
        /// GET /api/ProjectMembers  
        /// Response: [ { "ProjectMemberId": 1, "MemberId": 4, "Role": "Admin", ... }, ... ]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectMember>>> GetProjectMembers()
        {
            return await _context.ProjectMembers.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific project member by ID.
        /// </summary>
        /// <param name="id">The ID of the project member.</param>
        /// <returns>The project member with the specified ID.</returns>
        /// <example>
        /// GET /api/ProjectMembers/5  
        /// Response: { "ProjectMemberId": 5, "MemberId": 4, "Role": "Member", ... }
        /// </example>
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

        /// <summary>
        /// Creates a new project member.
        /// </summary>
        /// <param name="projectMember">The ProjectMember object to create.</param>
        /// <returns>The created project member.</returns>
        /// <example>
        /// POST /api/ProjectMembers  
        /// Body: { "ProjectId": 1, "MemberId": 4, "Role": "Admin" }  
        /// Response: Created { "ProjectMemberId": 10, "ProjectId": 1, "MemberId": 4, "Role": "Admin", ... }
        /// </example>
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

        /// <summary>
        /// Updates an existing project member.
        /// </summary>
        /// <param name="id">The ID of the project member to update.</param>
        /// <param name="projectMember">The updated ProjectMember object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/ProjectMembers/5  
        /// Body: { "ProjectMemberId": 5, "ProjectId": 1, "MemberId": 4, "Role": "Co-Admin" }  
        /// Response: No Content
        /// </example>
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

        /// <summary>
        /// Deletes a specific project member.
        /// </summary>
        /// <param name="id">The ID of the project member to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/ProjectMembers/5  
        /// Response: No Content
        /// </example>
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
