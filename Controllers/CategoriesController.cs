using Microsoft.AspNetCore.Mvc;
using Blanca_eManagement.Data;
using Blanca_eManagement.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;


public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();

        var mainCategoriesCount = categories
            .Where(c => c.ParentCategoryId == null)
            .Select(c => new
            {
                Name = c.Name,
                Count = categories.Count(sub => sub.ParentCategoryId == c.Id)
            }).ToList();

        ViewData["MainCategoriesCount"] = mainCategoriesCount;

        return View(categories);
    }

    public IActionResult Categories(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
       
        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm));
        }

       
        var totalCategories = query.Count();
        var categories = query
            .OrderBy(c => c.Name) 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

       
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);
        ViewBag.SearchTerm = searchTerm;

     
        ViewBag.HasPreviousPage = pageNumber > 1;
        ViewBag.HasNextPage = pageNumber < ViewBag.TotalPages;

        return View(categories);
    }

    public IActionResult CategoriesManagement(string searchQuery = "", int pageNumber = 1, int pageSize = 8)
    {
        if (_context.Categories == null)
        {
            return View("~/Views/Admin/Categories.cshtml", new List<CategoryModel>());
        }

        // Filtrare după termenul de căutare 
        var query = _context.Categories.Include(c => c.ParentCategory).AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(c => c.Name.Contains(searchQuery));
        }

    
        int totalCategories = query.Count();

       
        var categories = query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

    
        ViewBag.TotalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.SearchQuery = searchQuery;

        return View("~/Views/Admin/Categories.cshtml", categories);
    }

    
    [HttpPost]
    public IActionResult AddCategory(string categoryName, int? parentCategoryId)
    {
        if (!string.IsNullOrEmpty(categoryName))
        {
            var category = new CategoryModel
            {
                Name = categoryName,
                ParentCategoryId = parentCategoryId
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        return RedirectToAction("Categories", "Admin");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        try
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            // Redirecționează către view-ul din /Views/Admin/Categories.cshtml
            return RedirectToAction("Categories", "Admin");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la ștergerea categoriei: {ex.Message}");
            return BadRequest("A apărut o eroare la ștergerea categoriei.");
        }

    }

    [HttpPost]
    public IActionResult DeleteBar(int id)
    {
        try
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Bar", "Categories");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la ștergerea categoriei: {ex.Message}");
            return BadRequest("A apărut o eroare la ștergerea categoriei.");
        }

    }

    [HttpPost]
    public IActionResult DeleteRestaurant(int id)
    {
        try
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Restaurant", "Categories");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la ștergerea categoriei: {ex.Message}");
            return BadRequest("A apărut o eroare la ștergerea categoriei.");
        }

    }
    public IActionResult Restaurant()
    {
        var subcategories = _context.Categories
            .Where(c => c.ParentCategory != null && c.ParentCategory.Name == "Restaurant")
            .ToList();

        return View(subcategories);
    }

    public IActionResult Bar()
    {
        var subcategories = _context.Categories
            .Where(c => c.ParentCategory != null && c.ParentCategory.Name == "Bar")
            .ToList();

        return View(subcategories);
    }

    [HttpPost]
    public async Task<IActionResult> AddSubcategoryBar(string CategoryName, string ParentCategoryName)
    {
        if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrEmpty(ParentCategoryName))
        {
            return RedirectToAction("Bar");
        }

        var parentCategory = _context.Categories.FirstOrDefault(c => c.Name == ParentCategoryName);
        if (parentCategory == null)
        {
            return RedirectToAction("Bar");
        }

        var subcategory = new CategoryModel
        {
            Name = CategoryName,
            ParentCategoryId = parentCategory.Id
        };

        _context.Categories.Add(subcategory);
        await _context.SaveChangesAsync();

        return RedirectToAction("Bar"); // Redirecționează la pagina Bar
    }

    [HttpPost]
    public async Task<IActionResult> AddSubcategoryRestaurant(string CategoryName, string ParentCategoryName)
    {
        if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrEmpty(ParentCategoryName))
        {
            return RedirectToAction("Restaurant"); // Redirecționează la pagina Bar în cazul unui input invalid
        }

        // Găsește categoria principală după nume
        var parentCategory = _context.Categories.FirstOrDefault(c => c.Name == ParentCategoryName);
        if (parentCategory == null)
        {
            return RedirectToAction("Restaurant"); // Redirecționează dacă categoria principală nu există
        }

     
        var subcategory = new CategoryModel
        {
            Name = CategoryName,
            ParentCategoryId = parentCategory.Id
        };

        _context.Categories.Add(subcategory);
        await _context.SaveChangesAsync();

        return RedirectToAction("Restaurant");
    }

    [HttpPost]
    public IActionResult EditCategory(CategoryModel model)
    {
        var category = _context.Categories.Find(model.Id);
        if (category != null)
        {
            category.Name = model.Name;
            category.ParentCategoryId = model.ParentCategoryId;
            _context.SaveChanges();
        }
        return RedirectToAction("Categories", "Admin");
    }

    [HttpPost]
    public IActionResult EditSubcategoryBar(int CategoryId, string CategoryName)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == CategoryId);
        if (category != null)
        {
            category.Name = CategoryName;
            _context.SaveChanges();
        }
        return RedirectToAction("Bar", "Categories");
    }

    public IActionResult EditSubcategoryRestaurant(int CategoryId, string CategoryName)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == CategoryId);
        if (category != null)
        {
            category.Name = CategoryName;
            _context.SaveChanges();
        }
        return RedirectToAction("Restaurant", "Categories");
    }

}