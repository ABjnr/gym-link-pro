using System.Linq;
using System.Threading.Tasks;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymLinkPro.Controllers
{
    [Authorize]
    public class ProjectsViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all projects including their creators.
        /// </summary>
        /// <returns>The index view with a list of projects.</returns>
        /// <example>
        /// GET /ProjectsView/Index  
        /// Response: [HTML page with table of projects]
        /// </example>
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Creator)
                .ToListAsync();
            return View(projects);
        }

        /// <summary>
        /// Displays the details for a specific project.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The details view for the specified project.</returns>
        /// <example>
        /// GET /ProjectsView/Details/5  
        /// Response: [HTML page with project details]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null) return NotFound();
            return View(project);
        }

        /// <summary>
        /// Displays the create project form.
        /// </summary>
        /// <returns>The create view for a new project.</returns>
        /// <example>
        /// GET /ProjectsView/Create  
        /// Response: [HTML form for creating a project]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View();
        }

        /// <summary>
        /// Handles creation of a new project.
        /// </summary>
        /// <param name="project">The Project object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectsView/Create  
        /// Body: { "Name": "New Project", "Description": "Project description", "CreatorId": 3 }  
        /// Response: Redirect to /ProjectsView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View(project);
        }

        /// <summary>
        /// Displays the edit form for a specific project.
        /// </summary>
        /// <param name="id">The ID of the project to edit.</param>
        /// <returns>The edit view for the specified project.</returns>
        /// <example>
        /// GET /ProjectsView/Edit/5  
        /// Response: [HTML form for editing a project]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View(project);
        }

        /// <summary>
        /// Handles updates to a project.
        /// </summary>
        /// <param name="id">The ID of the project to update.</param>
        /// <param name="project">The updated Project object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectsView/Edit/5  
        /// Body: { "ProjectId": 5, "Name": "Updated Project", "Description": "Updated description", "CreatorId": 3 }  
        /// Response: Redirect to /ProjectsView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View(project);
        }

        /// <summary>
        /// Displays the delete confirmation page for a specific project.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        /// <returns>The delete view for the specified project.</returns>
        /// <example>
        /// GET /ProjectsView/Delete/5  
        /// Response: [HTML confirmation page for deleting a project]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null) return NotFound();
            return View(project);
        }

        /// <summary>
        /// Handles deletion of a project.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /ProjectsView/Delete/5  
        /// Response: Redirect to /ProjectsView/Index
        /// </example>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}