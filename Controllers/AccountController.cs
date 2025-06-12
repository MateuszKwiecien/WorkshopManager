using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;
using WorkshopManager.ViewModels;

namespace WorkshopManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> user, SignInManager<ApplicationUser> signIn)
        {
            _userManager  = user;
            _signInManager = signIn;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // domyślnie przypisz Recepcjonistę – zmień wg potrzeb
                await _userManager.AddToRoleAsync(user, "Recepcjonista");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
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
