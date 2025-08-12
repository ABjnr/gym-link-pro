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

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Creator)
                .ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null) return NotFound();
            return View(project);
        }

        public IActionResult Create()
        {
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            ViewBag.Users = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View(project);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null) return NotFound();
            return View(project);
        }

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