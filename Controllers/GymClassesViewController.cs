using Microsoft.AspNetCore.Http;
using System.IO;
using System;
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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.GymClasses.CountAsync();
            var gymClasses = await _context.GymClasses
                .OrderBy(c => c.GymClassId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
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
        /// 
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Trainers = _context.Users
                .Where(u => u.Role == "Trainer")
                .Select(u => new { u.UserId, Name = u.FirstName + " " + u.LastName })
                .ToList();
            return View();
        }

        /// <summary>
        /// Handles creation of a new gym class including image upload.
        /// </summary>
        /// <param name="gymClass">The GymClass object to create.</param>
        /// <param name="imageFile">The image file uploaded.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /GymClassesView/Create  
        /// Body: { "Name": "Yoga", "StartTime": "09:00", "EndTime": "10:00", "TrainerId": 2, ... }  
        /// Response: Redirect to /GymClassesView/Index
        /// </example>
        /// 

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymClass gymClass, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "gymclasses");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    gymClass.ImagePath = "/images/gymclasses/" + fileName; 
                }

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
        /// 
        [Authorize(Roles = "Admin")]
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
        /// Handles updates to a gym class, including updating the image if a new file is provided.
        /// </summary>
        /// <param name="id">The ID of the gym class to update.</param>
        /// <param name="gymClass">The updated GymClass object.</param>
        /// <param name="imageFile">The new image file (if any)</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /GymClassesView/Edit/5  
        /// Body: { "GymClassId": 5, "Name": "Pilates", "TrainerId": 2, ... }  
        /// Response: Redirect to /GymClassesView/Index
        /// </example>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymClass gymClass, IFormFile imageFile)
        {
            if (id != gymClass.GymClassId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "gymclasses");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    gymClass.ImagePath = "/images/gymclasses/" + fileName;
                }

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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