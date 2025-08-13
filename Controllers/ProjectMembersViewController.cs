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
    public class ProjectMembersViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectMembersViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all project members.
        /// </summary>
        /// <returns>The index view with a list of project members.</returns>
        /// <example>
        /// GET /ProjectMembersView/Index  
        /// Response: [HTML page with table of project members and their roles]
        /// </example>
        public async Task<IActionResult> Index()
        {
            var projectMembers = await _context.ProjectMembers.ToListAsync();
            ViewBag.Users = _context.Users.ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Projects = _context.Projects.ToDictionary(p => p.ProjectId, p => p.Name);
            return View(projectMembers);
        }

        /// <summary>
        /// Displays the details for a specific project member.
        /// </summary>
        /// <param name="id">The ID of the project member.</param>
        /// <returns>The details view for the specified project member.</returns>
        /// <example>
        /// GET /ProjectMembersView/Details/5  
        /// Response: [HTML page with details of the project member]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();

            ViewBag.Users = _context.Users.ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Projects = _context.Projects.ToDictionary(p => p.ProjectId, p => p.Name);

            return View(member);
        }

        /// <summary>
        /// Displays the create project member form.
        /// </summary>
        /// <returns>The create view for a new project member.</returns>
        /// <example>
        /// GET /ProjectMembersView/Create  
        /// Response: [HTML form for creating a project member]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            ViewBag.Roles = new System.Collections.Generic.List<string> { "Admin", "Member" };
            return View();
        }

        /// <summary>
        /// Handles creation of a new project member.
        /// </summary>
        /// <param name="member">The ProjectMember object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectMembersView/Create  
        /// Body: { "ProjectId": 1, "MemberId": 4, "Role": "Member" }  
        /// Response: Redirect to /ProjectMembersView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectMember member)
        {
            if (ModelState.IsValid)
            {
                _context.ProjectMembers.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            ViewBag.Roles = new System.Collections.Generic.List<string> { "Admin", "Member" };
            return View(member);
        }

        /// <summary>
        /// Displays the edit form for a project member.
        /// </summary>
        /// <param name="id">The ID of the project member to edit.</param>
        /// <returns>The edit view for the specified project member.</returns>
        /// <example>
        /// GET /ProjectMembersView/Edit/5  
        /// Response: [HTML form for editing a project member]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();

            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();

            // Only users already assigned to the selected project
            ViewBag.Users = _context.ProjectMembers
                .Where(pm => pm.ProjectId == member.ProjectId)
                .Join(_context.Users, pm => pm.MemberId, u => u.UserId, (pm, u) => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            ViewBag.Roles = new System.Collections.Generic.List<string> { "Admin", "Member" };
            return View(member);
        }

        /// <summary>
        /// Handles updates to a project member.
        /// </summary>
        /// <param name="id">The ID of the project member to update.</param>
        /// <param name="member">The updated ProjectMember object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /ProjectMembersView/Edit/5  
        /// Body: { "ProjectMemberId": 5, "ProjectId": 1, "MemberId": 4, "Role": "Admin" }  
        /// Response: Redirect to /ProjectMembersView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectMember member)
        {
            if (id != member.ProjectMemberId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Projects = _context.Projects
                .Select(p => new { p.ProjectId, p.Name })
                .ToList();

            ViewBag.Users = _context.ProjectMembers
                .Where(pm => pm.ProjectId == member.ProjectId)
                .Join(_context.Users, pm => pm.MemberId, u => u.UserId, (pm, u) => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            ViewBag.Roles = new System.Collections.Generic.List<string> { "Admin", "Member" };
            return View(member);
        }

        /// <summary>
        /// Displays the delete confirmation page for a project member.
        /// </summary>
        /// <param name="id">The ID of the project member to delete.</param>
        /// <returns>The delete view for the specified project member.</returns>
        /// <example>
        /// GET /ProjectMembersView/Delete/5  
        /// Response: [HTML confirmation page for deletion]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();

            ViewBag.Users = _context.Users.ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Projects = _context.Projects.ToDictionary(p => p.ProjectId, p => p.Name);

            return View(member);
        }

        /// <summary>
        /// Handles deletion of a project member.
        /// </summary>
        /// <param name="id">The ID of the project member to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /ProjectMembersView/Delete/5  
        /// Response: Redirect to /ProjectMembersView/Index
        /// </example>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member != null)
            {
                _context.ProjectMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}