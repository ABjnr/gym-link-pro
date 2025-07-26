using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectLink>> PostProjectLink(ProjectLink projectLink)
        {
            _context.ProjectLinks.Add(projectLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectLink", new { id = projectLink.ProjectLinkId }, projectLink);
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

            _context.ProjectLinks.Remove(projectLink);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectLinkExists(int id)
        {
            return _context.ProjectLinks.Any(e => e.ProjectLinkId == id);
        }
    }
}
