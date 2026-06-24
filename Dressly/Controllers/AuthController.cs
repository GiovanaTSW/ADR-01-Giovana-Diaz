using System.Security.Claims;
using Dressly_MVC.Services;
using Dressly_MVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _auth;
    private readonly SeedService _seed;

    public AuthController(IAuthService auth, SeedService seed)
    {
        _auth = auth;
        _seed = seed;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var usuario = await _auth.LoginAsync(model.Email, model.Password);
        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = model.Recordarme });

        return RedirectToLocal(returnUrl);
    }

    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (exito, error, usuarioId) = await _auth.RegisterAsync(model.Nombre, model.Email, model.Password);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        await _seed.SeedUserDataAsync(usuarioId);
        return RedirectToAction(nameof(Login), new { returnUrl = "/" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }
}
