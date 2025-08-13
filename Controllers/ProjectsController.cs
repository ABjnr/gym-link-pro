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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <returns>An enumerable list of projects.</returns>
        /// <example>
        /// GET /api/Projects
        /// Response: [ { "ProjectId": 1, "Name": "Project A", ... }, ... ]
        /// </example>
        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific project by ID.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The project with the specified ID.</returns>
        /// <example>
        /// GET /api/Projects/5
        /// Response: { "ProjectId": 5, "Name": "Project B", ... }
        /// </example>
        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return project;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="project">The Project object to create.</param>
        /// <returns>The created project.</returns>
        /// <example>
        /// POST /api/Projects
        /// Body: { "Name": "New Project", "Description": "Project description" }
        /// Response: Created { "ProjectId": 10, "Name": "New Project", ... }
        /// </example>
        // POST: api/Projects
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            int currentUserId = GetCurrentUserId();
            project.CreatorId = currentUserId;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Add creator as ProjectMember with Admin role
            var adminMember = new ProjectMember
            {
                ProjectId = project.ProjectId,
                MemberId = currentUserId,
                Role = "Admin"
            };
            _context.ProjectMembers.Add(adminMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">The ID of the project to update.</param>
        /// <param name="project">The updated Project object.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// PUT /api/Projects/5
        /// Body: { "ProjectId": 5, "Name": "Updated Project", ... }
        /// Response: No Content
        /// </example>
        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            int currentUserId = GetCurrentUserId();
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == id && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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
        /// Deletes a specific project.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        /// <returns>No content on success.</returns>
        /// <example>
        /// DELETE /api/Projects/5
        /// Response: No Content
        /// </example>
        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            int currentUserId = GetCurrentUserId();
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == id && pm.MemberId == currentUserId);

            bool isAdminOrCoAdmin = membership != null &&
                (membership.Role == "Admin" || membership.Role == "Co-Admin");

            if (!isAdminOrCoAdmin)
            {
                return Forbid();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
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
