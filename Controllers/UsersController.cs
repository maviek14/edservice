using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpDeletingDataClient _httpDeletingDataClient;

        public UsersController(
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IHttpDeletingDataClient httpDeletingDataClient
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpDeletingDataClient = httpDeletingDataClient;
        }

        // GET: Users
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Index()
        {


            var users = _context.AppUsers.Include(u => u.Profile);
            if (User.IsInRole("User"))
            {
                string username = User.Identity.Name;
                int profileID = _context.Profiles.Single(_ => _.UserName.Equals(username)).ID;
                users = users.Where(_ => _.ProfileID == profileID).Include(u => u.Profile);
            }

            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.AppUsers
                .Include(u => u.Profile).Include(p => p.Profile.Contracts).Include(p => p.Profile.Devices)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
/*
        // GET: Users/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ProfileID"] = new SelectList(_context.Profiles, "ID", "UserName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,AddedTime,ProfileID")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfileID"] = new SelectList(_context.Profiles, "ID", "UserName", user.ProfileID);
            return View(user);
        }
*/
        // GET: Users/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.AppUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,ProfileID")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.FindByEmailAsync(_context.Profiles.Single(p => p.ID == _context.AppUsers.Single(u => u.ID == id).ProfileID).UserName);
            var username = _context.Profiles.Single(p => p.ID == _context.AppUsers.Single(u => u.ID == id).ProfileID).UserName;
            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            try
            {
                var obj = new SingleStringClass { Username = username };
                await _httpDeletingDataClient.DeleteUserData(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send synchronously: {ex.Message}");
            }

            Console.WriteLine($"User with ID {userId} was deleted.");

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.AppUsers.Any(e => e.ID == id);
        }
    }
}
