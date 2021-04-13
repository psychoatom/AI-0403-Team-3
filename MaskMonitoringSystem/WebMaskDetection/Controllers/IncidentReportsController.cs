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
    public class IncidentReportsController : Controller
    {
        private readonly DatabaseContext _context;

        public IncidentReportsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: IncidentReports
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.IncidentReport.OrderByDescending(x => x.IncidentDateTime).Include(i => i.LocationSetting);
            return View(await databaseContext.ToListAsync());
        }

        // GET: IncidentReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidentReport = await _context.IncidentReport
                .Include(i => i.LocationSetting)
                .SingleOrDefaultAsync(m => m.IncidentIndex == id);
            if (incidentReport == null)
            {
                return NotFound();
            }

            return View(incidentReport);
        }

        // GET: IncidentReports/Create
        public IActionResult Create()
        {
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "locationIndex");
            return View();
        }

        // POST: IncidentReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IncidentIndex,LocationIndex,IncidentDateTime,Note")] IncidentReport incidentReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(incidentReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "locationIndex", incidentReport.LocationIndex);
            return View(incidentReport);
        }

        // GET: IncidentReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidentReport = await _context.IncidentReport.SingleOrDefaultAsync(m => m.IncidentIndex == id);
            if (incidentReport == null)
            {
                return NotFound();
            }
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "Location", incidentReport.LocationIndex);
            return View(incidentReport);
        }

        // POST: IncidentReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IncidentIndex,LocationIndex,IncidentDateTime,Note")] IncidentReport incidentReport)
        {
            if (id != incidentReport.IncidentIndex)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incidentReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidentReportExists(incidentReport.IncidentIndex))
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
            ViewData["LocationIndex"] = new SelectList(_context.LocationSetting, "locationIndex", "locationIndex", incidentReport.LocationIndex);
            return View(incidentReport);
        }

        // GET: IncidentReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidentReport = await _context.IncidentReport
                .Include(i => i.LocationSetting)
                .SingleOrDefaultAsync(m => m.IncidentIndex == id);
            if (incidentReport == null)
            {
                return NotFound();
            }

            return View(incidentReport);
        }

        // POST: IncidentReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incidentReport = await _context.IncidentReport.SingleOrDefaultAsync(m => m.IncidentIndex == id);
            _context.IncidentReport.Remove(incidentReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncidentReportExists(int id)
        {
            return _context.IncidentReport.Any(e => e.IncidentIndex == id);
        }
    }
}
