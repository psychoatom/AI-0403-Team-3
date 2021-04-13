using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebMaskDetection.EntityStore;
using WebMaskDetection.Models;

namespace WebMaskDetection.Controllers
{
    public class LocationEmailsController : Controller
    {
        private readonly DatabaseContext _context;

        public LocationEmailsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: LocationEmails
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.LocationEmail.Include(l => l.LocationSetting);
            return View(await databaseContext.ToListAsync());
        }

        // GET: LocationEmails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEmail = await _context.LocationEmail
                .Include(l => l.LocationSetting)
                .SingleOrDefaultAsync(m => m.LE_Index == id);
            if (locationEmail == null)
            {
                return NotFound();
            }

            return View(locationEmail);
        }

        // GET: LocationEmails/Create
        public IActionResult Create()
        {
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location");
            return View();
        }

        // POST: LocationEmails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LE_Index,LocationIndex,Email")] LocationEmail locationEmail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationEmail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location", locationEmail.LocationIndex);
            return View(locationEmail);
        }

        // GET: LocationEmails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEmail = await _context.LocationEmail.SingleOrDefaultAsync(m => m.LE_Index == id);
            if (locationEmail == null)
            {
                return NotFound();
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location", locationEmail.LocationIndex);
            return View(locationEmail);
        }

        // POST: LocationEmails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LE_Index,LocationIndex,Email")] LocationEmail locationEmail)
        {
            if (id != locationEmail.LE_Index)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationEmail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationEmailExists(locationEmail.LE_Index))
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
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location", locationEmail.LocationIndex);
            return View(locationEmail);
        }

        // GET: LocationEmails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEmail = await _context.LocationEmail
                .Include(l => l.LocationSetting)
                .SingleOrDefaultAsync(m => m.LE_Index == id);
            if (locationEmail == null)
            {
                return NotFound();
            }

            return View(locationEmail);
        }

        // POST: LocationEmails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locationEmail = await _context.LocationEmail.SingleOrDefaultAsync(m => m.LE_Index == id);
            _context.LocationEmail.Remove(locationEmail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationEmailExists(int id)
        {
            return _context.LocationEmail.Any(e => e.LE_Index == id);
        }
    }
}
