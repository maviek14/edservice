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

namespace PlatformService.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="Admin, User")]
        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contracts = _context.Contracts.Include(c => c.Device).Include(c => c.Mandatory).Include(c => c.Principal);
            return View(await contracts.ToListAsync());
        }

        // GET: Contracts/Details/5
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contracts = await _context.Contracts
                .Include(c => c.Device)
                .Include(c => c.Mandatory)
                .Include(c => c.Principal)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contracts == null)
            {
                return NotFound();
            }

            return View(contracts);
        }

        // GET: Contracts/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewData["DeviceID"] = new SelectList(_context.Devices.Where(d=>d.Profile.UserName.Equals(User.Identity.Name)), "ID", "Name");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Price,DeviceID")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                Profile profile = _context.Profiles.Single(p => p.UserName.Equals(User.Identity.Name));
                contract.CompletedTime = null;
                contract.CreatedTime = DateTime.Now.Date;
                contract.Status = ContractStatus.Available;
                contract.Principal = profile;
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeviceID"] = new SelectList(_context.Devices.Where(d => d.Profile.UserName.Equals(User.Identity.Name)), "ID", "Name", contract.DeviceID);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["DeviceID"] = new SelectList(_context.Devices.Where(d => d.Profile.UserName.Equals(User.Identity.Name)), "ID", "Name", contract.DeviceID);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Price,DeviceID")] Contract contract)
        {
            if (id != contract.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contract.Principal = _context.Profiles.Single(p => p.UserName.Equals(User.Identity.Name));
                    contract.Status = ContractStatus.Available;
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ID))
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
            ViewData["DeviceID"] = new SelectList(_context.Devices.Where(d => d.Profile.UserName.Equals(User.Identity.Name)), "ID", "Name", contract.DeviceID);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Device)
                .Include(c => c.Mandatory)
                .Include(c => c.Principal)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Contract/Take/5
        [Authorize(Roles = "User")]
        public ActionResult Take(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _context.Contracts.Find(id).Mandatory = _context.Profiles.Single(p => p.UserName.Equals(User.Identity.Name));
            _context.Contracts.Find(id).Status = ContractStatus.Taken;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Contract/Complete/5
        [Authorize(Roles = "User")]
        public ActionResult Complete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _context.Contracts.Find(id).Status = ContractStatus.Completed;
            _context.Contracts.Find(id).CompletedTime = DateTime.Now.Date;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Contract/Cancel/5
        [Authorize(Roles = "User")]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                _context.Contracts.Find(id).MandatoryID = null;
                _context.Contracts.Find(id).Status = ContractStatus.Available;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Przechwycono wyjątek w metodzie Cancel: {ex.Message}");
            }
            return RedirectToAction("Index");
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ID == id);
        }
    }
}
