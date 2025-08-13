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

        /// <summary>
        /// Displays a list of all gym classes.
        /// </summary>
        /// <returns>The index view with a list of gym classes.</returns>
        /// <example>
        /// GET /GymClassesView/Index  
        /// Response: [HTML page with table of gym classes]
        /// </example>
        public async Task<IActionResult> Index()
        {
            var gymClasses = await _context.GymClasses.ToListAsync();
            var trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);
            ViewBag.Trainers = trainers;
            return View(gymClasses);
        }

        /// <summary>
        /// Displays the details for a specific gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class.</param>
        /// <returns>The details view for the specified gym class.</returns>
        /// <example>
        /// GET /GymClassesView/Details/5  
        /// Response: [HTML page with gym class details]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();
            return View(gymClass);
        }

        /// <summary>
        /// Displays the create gym class form.
        /// </summary>
        /// <returns>The create view for a new gym class.</returns>
        /// <example>
        /// GET /GymClassesView/Create  
        /// Response: [HTML form for creating a gym class]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View();
        }

        /// <summary>
        /// Handles creation of a new gym class.
        /// </summary>
        /// <param name="gymClass">The GymClass object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /GymClassesView/Create  
        /// Body: { "Name": "Yoga", "StartTime": "09:00", "EndTime": "10:00", "TrainerId": 2, ... }  
        /// Response: Redirect to /GymClassesView/Index
        /// </example>
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

        /// <summary>
        /// Displays the edit form for a gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to edit.</param>
        /// <returns>The edit view for the specified gym class.</returns>
        /// <example>
        /// GET /GymClassesView/Edit/5  
        /// Response: [HTML form for editing a gym class]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();

            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            return View(gymClass);
        }

        /// <summary>
        /// Handles updates to a gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to update.</param>
        /// <param name="gymClass">The updated GymClass object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /GymClassesView/Edit/5  
        /// Body: { "GymClassId": 5, "Name": "Pilates", "TrainerId": 2, ... }  
        /// Response: Redirect to /GymClassesView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymClass gymClass)
        {
            if (id != gymClass.GymClassId) return NotFound();
            if (ModelState.IsValid)
            {
                var trainer = await _context.Users.FindAsync(gymClass.TrainerId);
                gymClass.Instructor = trainer != null ? $"{trainer.FirstName} {trainer.LastName}" : null;

                _context.Update(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();

            return View(gymClass);
        }

        /// <summary>
        /// Displays the delete confirmation page for a gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to delete.</param>
        /// <returns>The delete view for the specified gym class.</returns>
        /// <example>
        /// GET /GymClassesView/Delete/5  
        /// Response: [HTML confirmation page for deletion]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();
            return View(gymClass);
        }

        /// <summary>
        /// Handles deletion of a gym class.
        /// </summary>
        /// <param name="id">The ID of the gym class to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /GymClassesView/Delete/5  
        /// Response: Redirect to /GymClassesView/Index
        /// </example>
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