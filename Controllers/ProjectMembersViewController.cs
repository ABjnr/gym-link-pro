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

        public async Task<IActionResult> Index()
        {
            var projectMembers = await _context.ProjectMembers.ToListAsync();
            ViewBag.Users = _context.Users.ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Projects = _context.Projects.ToDictionary(p => p.ProjectId, p => p.Name);
            return View(projectMembers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        public IActionResult Create()
        {
            return View();
        }

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
            return View(member);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

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
            return View(member);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

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