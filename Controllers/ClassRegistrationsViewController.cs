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

        /// <summary>
        /// Displays a list of all class registrations.
        /// </summary>
        /// <returns>The index view with a list of class registrations.</returns>
        /// <example>
        /// GET /ClassRegistrationsView/Index  
        /// Response: [HTML page with table of class registrations]
        /// </example>
        public async Task<IActionResult> Index()
        {
            ViewBag.MemberNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.ClassNames = _context.GymClasses
                .ToDictionary(c => c.GymClassId, c => c.Name);

            return View(await _context.ClassRegistrations.ToListAsync());
        }

        /// <summary>
        /// Displays the details for a specific class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration.</param>
        /// <returns>The details view for the specified class registration.</returns>
        /// <example>
        /// GET /ClassRegistrationsView/Details/5  
        /// Response: [HTML page with details of the class registration]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();

            ViewBag.MemberNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.ClassNames = _context.GymClasses
                .ToDictionary(c => c.GymClassId, c => c.Name);

            return View(registration);
        }

        /// <summary>
        /// Displays the create class registration form with dropdowns for member, class, and status.
        /// </summary>
        /// <returns>The create view for a new class registration, with dropdowns for Member, Class, and Status (Pending, Confirmed, Canceled).</returns>
        /// <example>
        /// GET /ClassRegistrationsView/Create  
        /// Response: [HTML form for creating a class registration with dropdowns]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Members = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            ViewBag.Classes = _context.GymClasses
                .Select(c => new { c.GymClassId, c.Name })
                .ToList();
            ViewBag.Statuses = new List<string> { "Pending", "Confirmed", "Canceled" };
            return View();
        }

        /// <summary>
        /// Handles creation of a new class registration.
        /// </summary>
        /// <param name="registration">The ClassRegistration object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form with dropdowns for Member, Class, and Status.</returns>
        /// <example>
        /// POST /ClassRegistrationsView/Create  
        /// Body: { "MemberId": 2, "ClassId": 5, "Status": "Pending", "RegistrationDate": "2023-04-01T00:00:00" }  
        /// Response: Redirect to /ClassRegistrationsView/Index
        /// </example>
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
            ViewBag.Members = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            ViewBag.Classes = _context.GymClasses
                .Select(c => new { c.GymClassId, c.Name })
                .ToList();
            ViewBag.Statuses = new List<string> { "Pending", "Confirmed", "Canceled" };
            return View(registration);
        }

        /// <summary>
        /// Displays the edit form for a specific class registration with dropdowns for member, class, and status.
        /// </summary>
        /// <param name="id">The ID of the class registration to edit.</param>
        /// <returns>The edit view for the specified class registration, with dropdowns for Member, Class, and Status (Pending, Confirmed, Canceled).</returns>
        /// <example>
        /// GET /ClassRegistrationsView/Edit/5  
        /// Response: [HTML form for editing a class registration with dropdowns]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();
            ViewBag.Members = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            ViewBag.Classes = _context.GymClasses
                .Select(c => new { c.GymClassId, c.Name })
                .ToList();
            ViewBag.Statuses = new List<string> { "Pending", "Confirmed", "Canceled" };
            return View(registration);
        }

        /// <summary>
        /// Handles updates to a class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration to update.</param>
        /// <param name="registration">The updated ClassRegistration object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form with dropdowns for Member, Class, and Status.</returns>
        /// <example>
        /// POST /ClassRegistrationsView/Edit/5  
        /// Body: { "ClassRegistrationId": 5, "MemberId": 2, "ClassId": 5, "Status": "Confirmed", "RegistrationDate": "2023-04-02T00:00:00" }  
        /// Response: Redirect to /ClassRegistrationsView/Index
        /// </example>
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
            ViewBag.Members = _context.Users
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            ViewBag.Classes = _context.GymClasses
                .Select(c => new { c.GymClassId, c.Name })
                .ToList();
            ViewBag.Statuses = new List<string> { "Pending", "Confirmed", "Canceled" };
            return View(registration);
        }

        /// <summary>
        /// Displays the delete confirmation page for a class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration to delete.</param>
        /// <returns>The delete view for the specified class registration.</returns>
        /// <example>
        /// GET /ClassRegistrationsView/Delete/5  
        /// Response: [HTML confirmation page for deletion]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _context.ClassRegistrations.FindAsync(id);
            if (registration == null) return NotFound();

            ViewBag.MemberNames = _context.Users
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.ClassNames = _context.GymClasses
                .ToDictionary(c => c.GymClassId, c => c.Name);

            return View(registration);
        }

        /// <summary>
        /// Handles deletion of a class registration.
        /// </summary>
        /// <param name="id">The ID of the class registration to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /ClassRegistrationsView/Delete/5  
        /// Response: Redirect to /ClassRegistrationsView/Index
        /// </example>
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