using System.Threading.Tasks;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GymLinkPro.Controllers
{
    [Authorize]
    public class ProjectLinksViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectLinksViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all project links.
        /// </summary>
        /// <returns>The index view with a list of project links.</returns>
        /// <example>
        /// GET /ProjectLinksView/Index
        /// Response: [HTML page with table of project links]
        /// </example>
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.ProjectLinks.CountAsync();
            var links = await _context.ProjectLinks
                .OrderBy(l => l.ProjectLinkId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.UserNames = _context.Users.ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);

            return View(links);
        }

        /// <summary>
        /// Displays the details for a specific project link.
        /// </summary>
        /// <param name="id">The ID of the project link.</param>
        /// <returns>The details view for the specified project link.</returns>
        /// <example>
        /// GET /ProjectLinksView/Details/5
        /// Response: [HTML page with details of the project link]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        /// <summary>
        /// Displays the create project link form.
        /// </summary>
        /// <returns>The create view for a new project link.</returns>
        /// <example>
        /// GET /ProjectLinksView/Create
        /// Response: [HTML form for creating a project link]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Projects = _context.Projects.Select(p => new { p.ProjectId, p.Name }).ToList();
            ViewBag.UserNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            return View();
        }

        /// <summary>
        /// Handles creation of a new project link.
        /// </summary>
        /// <param name="link">The ProjectLink object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectLinksView/Create  
        /// Body: { "ProjectId": 1, "Url": "https://example.com", "Description": "Sample", "Category": "Docs" }  
        /// Response: Redirect to /ProjectLinksView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectLink link)
        {
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.FindAsync(link.ProjectId);
                if (project != null)
                {
                    link.AddedByUserId = project.CreatorId;
                }
                _context.ProjectLinks.Add(link);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Projects = _context.Projects.Select(p => new { p.ProjectId, p.Name }).ToList();
            ViewBag.UserNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            return View(link);
        }

        /// <summary>
        /// Displays the edit form for a project link.
        /// </summary>
        /// <param name="id">The ID of the project link to edit.</param>
        /// <returns>The edit view for the specified project link.</returns>
        /// <example>
        /// GET /ProjectLinksView/Edit/5  
        /// Response: [HTML form for editing a project link]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();
            ViewBag.UserNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            return View(link);
        }

        /// <summary>
        /// Handles updates to a project link.
        /// </summary>
        /// <param name="id">The ID of the project link to update.</param>
        /// <param name="link">The updated ProjectLink object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectLinksView/Edit/5  
        /// Body: { "ProjectId": 1, "Url": "https://example.com", "Description": "Updated", "Category": "Docs" }  
        /// Response: Redirect to /ProjectLinksView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectLink link)
        {
            if (id != link.ProjectLinkId) return NotFound();
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.FindAsync(link.ProjectId);
                if (project != null)
                {
                    link.AddedByUserId = project.CreatorId;
                }
                _context.Update(link);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();
            ViewBag.UserNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            return View(link);
        }

        /// <summary>
        /// Displays the delete confirmation page for a project link.
        /// </summary>
        /// <param name="id">The ID of the project link to delete.</param>
        /// <returns>The delete view for the specified project link.</returns>
        /// <example>
        /// GET /ProjectLinksView/Delete/5  
        /// Response: [HTML confirmation page for deletion]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        /// <summary>
        /// Handles deletion of a project link.
        /// </summary>
        /// <param name="id">The ID of the project link to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /ProjectLinksView/Delete/5  
        /// Response: Redirect to /ProjectLinksView/Index
        /// </example>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link != null)
            {
                _context.ProjectLinks.Remove(link);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}