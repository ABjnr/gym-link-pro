using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymLinkPro.Controllers
{
    [Authorize]
    public class ClassRegistrationsViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassRegistrationsViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ClassRegistrations.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassRegistration registration)
        {
            if (ModelState.IsValid)
            {
                _context.ClassRegistrations.Add(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassRegistration registration)
        {
            if (id != registration.ClassRegistrationId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration != null)
            {
                _context.ClassRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}