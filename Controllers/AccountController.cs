using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.Models;
using WorkshopManager.ViewModels;

namespace WorkshopManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public AccountController(UserManager<ApplicationUser>  userMgr,
            SignInManager<ApplicationUser> signInMgr,
            RoleManager<IdentityRole>      roleMgr)
        {
            _userMgr  = userMgr;
            _signInManager = signInMgr;
            _roleMgr   = roleMgr;
        }

/*──── GET: /Account/Register ────*/
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.RoleList = new SelectList(new[] { "Admin", "Mechanik", "Recepcjonista" });
            return View();
        }

/*──── POST: /Account/Register ────*/
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleList = new SelectList(new[] { "Admin", "Mechanik", "Recepcjonista" }, vm.Role);
                return View(vm);
            }

            // upewnij się, że podana rola istnieje
            if (!await _roleMgr.RoleExistsAsync(vm.Role))
            {
                ModelState.AddModelError("Role", "Wybrana rola nie istnieje.");
                ViewBag.RoleList = new SelectList(new[] { "Admin", "Mechanik", "Recepcjonista" }, vm.Role);
                return View(vm);
            }

            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var result = await _userMgr.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _userMgr.AddToRoleAsync(user, vm.Role);          // ← przypisz rolę
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewBag.RoleList = new SelectList(new[] { "Admin", "Mechanik", "Recepcjonista" }, vm.Role);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Nieprawidłowe dane logowania");
            return View(vm);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
