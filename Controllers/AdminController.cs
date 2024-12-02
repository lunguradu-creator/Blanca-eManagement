using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Blanca_eManagement.Data;
using Blanca_eManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Admin
    public IActionResult Index()
    {
        return View("Admin");
    }
    //PRODUSE
    [HttpGet]
    public IActionResult ProductManagement()
    {
        // Preluăm lista produselor din baza de date
        var products = _context.Products
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category, 
                Stock = p.Stock
            })
            .ToList();

       return View(products);
    }


    [HttpGet]
    public IActionResult CreateProduct()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult CreateProduct(ProductModel product)
    {
        if (ModelState.IsValid)
        {
           SaveProductToDatabase(product);
            return RedirectToAction("CreateProduct");
        }

     ViewBag.Categories = GetCategories(); 
        return View(product);
    }
    private List<CategoryModel> GetCategories()
    {
        
        return new List<CategoryModel>
        {
            new CategoryModel { Id = 1, Name = "Electronics" },
            new CategoryModel { Id = 2, Name = "Books" },
        };
    }

    private void SaveProductToDatabase(ProductModel product)
    {
        
    }
       public IActionResult CompanyManagement()
    {
        var model = new CompanyManagementViewModel
        {
            CompanyInfo = _context.Companies.FirstOrDefault() ?? new CompanyInfoModel(),
            VATCategories = _context.VATCategories.ToList() 
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult SaveCompany(CompanyInfoModel model)
    {
        var companyInfo = _context.Companies.FirstOrDefault();
        if (companyInfo == null)
        {
            _context.Companies.Add(model);
        }
        else
        {
            companyInfo.CompanyName = model.CompanyName;
            companyInfo.FiscalCode = model.FiscalCode;
            companyInfo.TradeRegisterNumber = model.TradeRegisterNumber;
            companyInfo.Address = model.Address;
            companyInfo.Bank = model.Bank;
            companyInfo.BankAccount = model.BankAccount;
            companyInfo.LegalRepresentative = model.LegalRepresentative;
            companyInfo.IsVATPayer = model.IsVATPayer;
            companyInfo.VATPercentage = model.VATPercentage;
        }

        _context.SaveChanges();
        return RedirectToAction("CompanyManagement");
    }

 
    [HttpPost]
    public IActionResult EditCompany()
    {
        var companyInfo = _context.Companies.FirstOrDefault();
        if (companyInfo != null)
        {
            companyInfo.IsEditMode = true;
            _context.SaveChanges();
        }
        return RedirectToAction("CompanyManagement");
    }


    [HttpPost]
    public IActionResult DeleteCompany()
    {
        var companyInfo = _context.Companies.FirstOrDefault();
        if (companyInfo != null)
        {
            _context.Companies.Remove(companyInfo);
            _context.SaveChanges();
        }
        return RedirectToAction("CompanyManagement");
    }

   
    public async Task<IActionResult> UsersManagement()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    
    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, role);
        }
        return RedirectToAction("UsersManagement");
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(string id, string userName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.UserName = userName;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("UsersManagement");
        }

        ModelState.AddModelError("", "Failed to update user.");
        return RedirectToAction("UsersManagement");
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("UsersManagement");
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete user.");
            }
        }
        return RedirectToAction("UsersManagement");
    }

    [HttpPost]
    public async Task<IActionResult> AssignRoleAndColor(string userId, string role, string color)
    {
       
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!string.IsNullOrEmpty(role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        user.Color = color;
        await _context.SaveChangesAsync();

        return RedirectToAction("UsersManagement");
    }


    public IActionResult VATCategories()
    {
        var categories = _context.VATCategories.ToList();
        return View(categories);
    }

    [HttpPost]
    public IActionResult AddVATCategory(VATCategory category)
    {
        if (ModelState.IsValid)
        {
            _context.VATCategories.Add(category);
            _context.SaveChanges();
        }
        return RedirectToAction("CompanyManagement");
    }

    [HttpPost]
    public IActionResult DeleteVATCategory(int id)
    {
        var category = _context.VATCategories.Find(id);
        if (category != null)
        {
            _context.VATCategories.Remove(category);
            _context.SaveChanges();
        }
        return RedirectToAction("CompanyManagement");
    }

    public IActionResult Categories()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }
}