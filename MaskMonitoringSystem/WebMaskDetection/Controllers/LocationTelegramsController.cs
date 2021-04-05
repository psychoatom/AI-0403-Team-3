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
    public class LocationTelegramsController : Controller
    {
        private readonly DatabaseContext _context;

        public LocationTelegramsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: LocationTelegrams
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.LocationTelegram.Include(l => l.LocationSetting);
            return View(await databaseContext.ToListAsync());
        }

        // GET: LocationTelegrams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationTelegram = await _context.LocationTelegram
                .Include(l => l.LocationSetting)
                .SingleOrDefaultAsync(m => m.LT_Index == id);
            if (locationTelegram == null)
            {
                return NotFound();
            }

            return View(locationTelegram);
        }

        // GET: LocationTelegrams/Create
        public IActionResult Create()
        {
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location");
            return View();
        }

        // POST: LocationTelegrams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LT_Index,LocationIndex,ChatID")] LocationTelegram locationTelegram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationTelegram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "location", locationTelegram.LocationIndex);
            return View(locationTelegram);
        }

        // GET: LocationTelegrams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationTelegram = await _context.LocationTelegram.SingleOrDefaultAsync(m => m.LT_Index == id);
            if (locationTelegram == null)
            {
                return NotFound();
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "location", locationTelegram.LocationIndex);
            return View(locationTelegram);
        }

        // POST: LocationTelegrams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LT_Index,LocationIndex,ChatID")] LocationTelegram locationTelegram)
        {
            if (id != locationTelegram.LT_Index)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationTelegram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationTelegramExists(locationTelegram.LT_Index))
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
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "location", locationTelegram.LocationIndex);
            return View(locationTelegram);
        }

        // GET: LocationTelegrams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationTelegram = await _context.LocationTelegram
                .Include(l => l.LocationSetting)
                .SingleOrDefaultAsync(m => m.LT_Index == id);
            if (locationTelegram == null)
            {
                return NotFound();
            }

            return View(locationTelegram);
        }

        // POST: LocationTelegrams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locationTelegram = await _context.LocationTelegram.SingleOrDefaultAsync(m => m.LT_Index == id);
            _context.LocationTelegram.Remove(locationTelegram);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationTelegramExists(int id)
        {
            return _context.LocationTelegram.Any(e => e.LT_Index == id);
        }
    }
}
