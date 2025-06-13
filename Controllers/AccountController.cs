using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.Models;
using WorkshopManager.ViewModels;

namespace WorkshopManager.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly SignInManager<ApplicationUser> _signInMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;
    private readonly ILogger<AccountController> _log;

    public AccountController(UserManager<ApplicationUser>  userMgr,
                             SignInManager<ApplicationUser> signInMgr,
                             RoleManager<IdentityRole>      roleMgr,
                             ILogger<AccountController>      log)
    {
        _userMgr  = userMgr;
        _signInMgr = signInMgr;
        _roleMgr   = roleMgr;
        _log       = log;
    }

    // GET /Account/Register
    public IActionResult Register() => View();

    // POST /Account/Register
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var result = await _userMgr.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _signInMgr.SignInAsync(user, isPersistent: false);
                _log.LogInformation("User {Email} registered", vm.Email);
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);

            return View(vm);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Register failed for {Email}", vm.Email);
            return View("Error");
        }
    }

    // GET /Account/Login
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST /Account/Login
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var result = await _signInMgr.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
            if (result.Succeeded)
            {
                _log.LogInformation("User {Email} logged in", vm.Email);
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            ModelState.AddModelError(string.Empty, "Nieprawidłowe dane logowania");
            return View(vm);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Login failed for {Email}", vm.Email);
            return View("Error");
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInMgr.SignOutAsync();
            _log.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Logout failed");
            return View("Error");
        }
    }
}
