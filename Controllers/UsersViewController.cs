using System.Threading.Tasks;
using GymLinkPro.Data;
using GymLinkPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace GymLinkPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all users.
        /// </summary>
        /// <returns>The index view with a list of users.</returns>
        /// <example>
        /// GET /UsersView/Index  
        /// Response: [HTML page with table of users]
        /// </example>
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Users.CountAsync();
            var users = await _context.Users
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(users);
        }

        /// <summary>
        /// Displays the details for a specific user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The details view for the specified user.</returns>
        /// <example>
        /// GET /UsersView/Details/5  
        /// Response: [HTML page with details of the user]
        /// </example>
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        /// <summary>
        /// Displays the create user form.
        /// </summary>
        /// <returns>The create view for a new user.</returns>
        /// <example>
        /// GET /UsersView/Create  
        /// Response: [HTML form for creating a user]
        /// </example>
        public IActionResult Create()
        {
            ViewBag.Roles = new List<string> { "Member", "Admin", "Trainer" };
            return View();
        }

        /// <summary>
        /// Handles creation of a new user.
        /// </summary>
        /// <param name="user">The User object to create.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /UsersView/Create  
        /// Body: { "FirstName": "John", "LastName": "Doe", "Email": "john.doe@example.com", "Role": "Member" }  
        /// Response: Redirect to /UsersView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new List<string> { "Member", "Admin", "Trainer" };
            return View(user);
        }

        /// <summary>
        /// Displays the edit form for a user.
        /// </summary>
        /// <param name="id">The ID of the user to edit.</param>
        /// <returns>The edit view for the specified user.</returns>
        /// <example>
        /// GET /UsersView/Edit/5  
        /// Response: [HTML form for editing a user]
        /// </example>
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            ViewBag.Roles = new List<string> { "Member", "Admin", "Trainer" };
            return View(user);
        }

        /// <summary>
        /// Handles updates to a user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="user">The updated User object.</param>
        /// <returns>Redirects to the index view on success, otherwise redisplays the form.</returns>
        /// <example>
        /// POST /UsersView/Edit/5  
        /// Body: { "UserId": 5, "FirstName": "Jane", "LastName": "Doe", "Role": "Admin" }  
        /// Response: Redirect to /UsersView/Index
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new List<string> { "Member", "Admin", "Trainer" };
            return View(user);
        }

        /// <summary>
        /// Displays the delete confirmation page for a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>The delete view for the specified user.</returns>
        /// <example>
        /// GET /UsersView/Delete/5  
        /// Response: [HTML confirmation page for deletion]
        /// </example>
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        /// <summary>
        /// Handles deletion of a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Redirects to the index view after deletion.</returns>
        /// <example>
        /// POST /UsersView/Delete/5  
        /// Response: Redirect to /UsersView/Index
        /// </example>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}