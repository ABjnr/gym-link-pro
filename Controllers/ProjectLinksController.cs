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

        /// <summary>
        /// Retrieves all project links.
        /// </summary>
        /// <returns>An enumerable list of project links.</returns>
        /// <example>
        /// GET /api/ProjectLinks  
        /// Response: [ { "ProjectLinkId": 1, "ProjectId": 1, "Url": "https://...", ... }, ... ]
        /// </example>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectLink>>> GetProjectLinks()
        {
            return await _context.ProjectLinks.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific project link by ID.
        /// </summary>
        /// <param name="id">The ID of the project link.</param>
        /// <returns>The project link with the specified ID.</returns>
        /// <example>
        /// GET /api/ProjectLinks/5  
        /// Response: { "ProjectLinkId": 5, "ProjectId": 1, "Url": "...", ... }
        /// </example>
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

        /// <summary>
        /// Updates an existing project link.
        /// </summary>
        /// <param name="id">The ID of the project link to update.</param>
        /// <param name="projectLink">The updated ProjectLink object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/ProjectLinks/5  
        /// Body: { "ProjectLinkId": 5, "ProjectId": 1, "Url": "https://new...", "Description": "Updated", "Category": "Docs" }  
        /// Response: No Content
        /// </example>
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

        /// <summary>
        /// Creates a new project link.
        /// </summary>
        /// <param name="projectLink">The ProjectLink object to create.</param>
        /// <returns>The created project link.</returns>
        /// <example>
        /// POST /api/ProjectLinks  
        /// Body: { "ProjectId": 1, "Url": "https://example.com", "Description": "New link", "Category": "Docs" }  
        /// Response: Created { "ProjectLinkId": 10, "ProjectId": 1, "Url": "https://example.com", ... }
        /// </example>
        [HttpPost]
        public async Task<ActionResult<ProjectLink>> PostProjectLink(ProjectLink projectLink)
        {
            projectLink.AddedByUserId = GetCurrentUserId();

            _context.ProjectLinks.Add(projectLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectLink), new { id = projectLink.ProjectLinkId }, projectLink);
        }

        /// <summary>
        /// Deletes a specific project link.
        /// </summary>
        /// <param name="id">The ID of the project link to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/ProjectLinks/5  
        /// Response: No Content
        /// </example>
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
