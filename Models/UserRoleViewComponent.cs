using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Blanca_eManagement.Models;

namespace Blanca_eManagement.ViewComponents
{
    public class UserRoleViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Content("No Role");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.Count > 0 ? roles[0] : "No Role";

            return Content(role);
        }
    }
}
