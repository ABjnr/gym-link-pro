using System.Threading.Tasks;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymLinkPro.Controllers
{
    [Authorize]
    public class GymClassesViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymClassesViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var gymClasses = await _context.GymClasses.ToListAsync();
            var trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Trainers = trainers;
            return View(gymClasses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();
            return View(gymClass);
        }

        public IActionResult Create()
        {
            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.GymClasses.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View(gymClass);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();
            return View(gymClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymClass gymClass)
        {
            if (id != gymClass.GymClassId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();
            return View(gymClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}