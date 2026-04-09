using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyPOSApp.Data;
using TinyPOSApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TinyPOSApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly TinyPOSContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(TinyPOSContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View("Edit", new Product 
            { 
                Id = Guid.NewGuid(), 
                Sku = $"SKU-{Random.Shared.Next(1000, 9999)}-01",
                ApplyBulkDiscount = true,
                TaxPercent = 0.00m,
                DiscountPercent = 0.00m
            });
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? uploadImage, bool removeImage = false)
        {
            if (ModelState.IsValid)
            {
                if (uploadImage != null && uploadImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(stream);
                    }
                    
                    product.ImageUrl = "/uploads/" + fileName;
                }
                else if (removeImage)
                {
                    product.ImageUrl = null;
                }

                var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);

                if (existingProduct == null)
                {
                    product.CreatedAt = DateTime.UtcNow;
                    _context.Add(product);
                }
                else
                {
                    // Retain existing image if new one wasn't uploaded AND wasn't removed
                    if (uploadImage == null && !removeImage && !string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }
                    
                    product.UpdatedAt = DateTime.UtcNow;
                    product.CreatedAt = existingProduct.CreatedAt;
                    
                    _context.Update(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
    }
}
