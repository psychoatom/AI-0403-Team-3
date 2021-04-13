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
    public class WebCamPredictSettingsController : Controller
    {
        private readonly DatabaseContext _context;

        public WebCamPredictSettingsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: WebCamPredictSettings
        public async Task<IActionResult> Index()
        {
            return View(await _context.WebCamPredictSetting.ToListAsync());
        }

        // GET: WebCamPredictSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webCamPredictSetting = await _context.WebCamPredictSetting
                .SingleOrDefaultAsync(m => m.wp_index == id);
            if (webCamPredictSetting == null)
            {
                return NotFound();
            }

            return View(webCamPredictSetting);
        }

        // GET: WebCamPredictSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WebCamPredictSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("wp_index,WebCamInterval,PredictionThreshold")] WebCamPredictSetting webCamPredictSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(webCamPredictSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(webCamPredictSetting);
        }

        // GET: WebCamPredictSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webCamPredictSetting = await _context.WebCamPredictSetting.SingleOrDefaultAsync(m => m.wp_index == id);
            if (webCamPredictSetting == null)
            {
                return NotFound();
            }
            return View(webCamPredictSetting);
        }

        // POST: WebCamPredictSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("wp_index,WebCamInterval,PredictionThreshold")] WebCamPredictSetting webCamPredictSetting)
        {
            if (id != webCamPredictSetting.wp_index)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(webCamPredictSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WebCamPredictSettingExists(webCamPredictSetting.wp_index))
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
            return View(webCamPredictSetting);
        }

        // GET: WebCamPredictSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webCamPredictSetting = await _context.WebCamPredictSetting
                .SingleOrDefaultAsync(m => m.wp_index == id);
            if (webCamPredictSetting == null)
            {
                return NotFound();
            }

            return View(webCamPredictSetting);
        }

        // POST: WebCamPredictSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var webCamPredictSetting = await _context.WebCamPredictSetting.SingleOrDefaultAsync(m => m.wp_index == id);
            _context.WebCamPredictSetting.Remove(webCamPredictSetting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WebCamPredictSettingExists(int id)
        {
            return _context.WebCamPredictSetting.Any(e => e.wp_index == id);
        }
    }
}
