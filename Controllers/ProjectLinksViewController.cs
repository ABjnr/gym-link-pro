using System.Threading.Tasks;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            return View(await _context.ProjectLinks.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectLink link)
        {
            if (ModelState.IsValid)
            {
                _context.ProjectLinks.Add(link);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(link);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectLink link)
        {
            if (id != link.ProjectLinkId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(link);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(link);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var link = await _context.ProjectLinks.FindAsync(id);
            if (link == null) return NotFound();
            return View(link);
        }

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