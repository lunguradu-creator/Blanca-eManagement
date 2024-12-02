using Microsoft.AspNetCore.Mvc;
using Blanca_eManagement.Data;
using System.Linq;
using Blanca_eManagement.Models;
using Microsoft.EntityFrameworkCore;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult ProductManagement(string searchQuery = "", int pageNumber = 1, int pageSize = 12)
    {
        if (_context.Products == null || _context.Categories == null)
        {
            return View("~/Views/Admin/ProductManagement.cshtml", new List<ProductModel>());
        }

        ViewBag.Categories = _context.Categories.ToList();

        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(p => p.Name.Contains(searchQuery));
        }

        ViewBag.TotalProducts = query.Count();

        var products = query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        int totalProducts = query.Count();
        ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.SearchQuery = searchQuery; // Preserve search term

        return View("~/Views/Admin/ProductManagement.cshtml", products);
    }


    [HttpPost]
    public IActionResult CreateProduct(ProductModel product)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Categoria selectată nu există.";
                    PopulateCategories();
                    return View("~/Views/Admin/CreateProduct.cshtml", product);
                }

                product.Category = category;

                _context.Products.Add(product);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Produsul a fost salvat cu succes.";
                return RedirectToAction("CreateProduct", "Admin");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Eroare la salvarea produsului: {ex.Message}";
            }
        }

        PopulateCategories();
        return View("~/Views/Admin/CreateProduct.cshtml", product);
    }

    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        PopulateCategories();
        return View("~/Views/Admin/EditProduct.cshtml", product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProduct(ProductModel product)
    {
        if (ModelState.IsValid)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Produsul a fost actualizat cu succes.";
                return RedirectToAction("ProductManagement");
            }
        }

        PopulateCategories();
        return View("~/Views/Admin/EditProduct.cshtml", product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        return RedirectToAction("ProductManagement");
    }

    private void PopulateCategories()
    {
        ViewBag.Categories = _context.Categories?.ToList() ?? new List<CategoryModel>();
    }

    public JsonResult GetSubcategories(int categoryId)
    {
        var subcategories = _context.Categories
            .Where(c => c.ParentCategoryId == categoryId)
            .Select(c => new { c.Id, c.Name })
            .ToList();

        return Json(subcategories);
    }
}
