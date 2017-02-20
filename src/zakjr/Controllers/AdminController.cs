﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zakjr.Data;
using zakjr.Models;

namespace zakjr.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            InitViewBag();
            return View(await _context.BlogPosts.ToListAsync());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var postModel = await _context.BlogPosts
                .Include(post => post.ContentList)
                .SingleOrDefaultAsync(post => post.ID == id);
            InitViewBag();
            return View(postModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var postToUpdate = await _context.BlogPosts
                .Include(post => post.ContentList)
                .SingleOrDefaultAsync(post => post.ID == id);
            postToUpdate.UpdatedDate = DateTime.Now;
            if (await TryUpdateModelAsync<BlogPost>(
                postToUpdate, "",
                post => post.Title,
                post => post.Subtitle,
                post => post.UpdatedDate,
                post => post.FeaturedImage,
                post => post.ContentList))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes :c");
                }
            }
            InitViewBag();
            return View(postToUpdate);
        }

        protected void InitViewBag()
        {
            ViewBag.LoggedIn = false;
            ViewBag.Username = "Zak";
        }
    }
}
