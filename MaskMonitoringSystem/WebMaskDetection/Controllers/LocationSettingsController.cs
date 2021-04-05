using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebMaskDetection.Models;
using WebMaskDetection.EntityStore;

namespace WebMaskDetection.Controllers
{
    public class LocationSettingsController : Controller
    {
        private readonly DatabaseContext _context;

        public LocationSettingsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: LocationSettings
        public async Task<IActionResult> Index()
        {
            return View(await _context.LocationSetting.ToListAsync());
        }

        // GET: LocationSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationSetting = await _context.LocationSetting
                .SingleOrDefaultAsync(m => m.locationIndex == id);
            if (locationSetting == null)
            {
                return NotFound();
            }

            return View(locationSetting);
        }

        // GET: LocationSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LocationSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("locationIndex,Location")] LocationSetting locationSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locationSetting);
        }

        // GET: LocationSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationSetting = await _context.LocationSetting.SingleOrDefaultAsync(m => m.locationIndex == id);
            if (locationSetting == null)
            {
                return NotFound();
            }
            return View(locationSetting);
        }

        // POST: LocationSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("locationIndex,Location")] LocationSetting locationSetting)
        {
            if (id != locationSetting.locationIndex)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationSettingExists(locationSetting.locationIndex))
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
            return View(locationSetting);
        }

        // GET: LocationSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationSetting = await _context.LocationSetting
                .SingleOrDefaultAsync(m => m.locationIndex == id);
            if (locationSetting == null)
            {
                return NotFound();
            }

            return View(locationSetting);
        }

        // POST: LocationSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locationSetting = await _context.LocationSetting.SingleOrDefaultAsync(m => m.locationIndex == id);
            _context.LocationSetting.Remove(locationSetting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationSettingExists(int id)
        {
            return _context.LocationSetting.Any(e => e.locationIndex == id);
        }
    }
}
