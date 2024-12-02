using Blanca_eManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class RecipesController : Controller
{
    private readonly ApplicationDbContext _context;
    public RecipesController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IActionResult RecipeManagement(int pageNumber = 1, int pageSize = 12)
    {
        var totalProducts = _context.Products.Count();
        var products = _context.Products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

        return View("~/Views/Admin/RecipeManagement.cshtml", products);
    }

[HttpPost]
    public IActionResult AddRecipe(int productId, string content)
    {
        var recipe = new RecipeModel
        {
            ProductId = productId,
            Content = content
        };

        _context.Recipes.Add(recipe);
        _context.SaveChanges();

        return RedirectToAction("RecipeManagement");
    }

[HttpPost]
    public IActionResult EditRecipe(int productId, string content)
    {
        var recipe = _context.Recipes.FirstOrDefault(r => r.ProductId == productId);
        if (recipe != null)
        {
            recipe.Content = content;
            _context.SaveChanges();
        }

        return RedirectToAction("RecipeManagement");
    }

    [HttpGet]
    public IActionResult GetRecipe(int productId)
    {
        var recipe = _context.Recipes
            .FirstOrDefault(r => r.ProductId == productId);

        if (recipe == null)
        {
            return Json(new { content = "" });
        }

        return Json(new { content = recipe.Content });
    }


    public IActionResult ViewRecipe(int productId)
    {
        var recipe = _context.Recipes.FirstOrDefault(r => r.ProductId == productId);
        return Json(recipe?.Content);
    }

}
