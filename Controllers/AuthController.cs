using Microsoft.AspNetCore.Mvc;
using OrderFlowMVC.Models;
using OrderFlowMVC.Services;

namespace OrderFlowMVC.Controllers;

public class AuthController : Controller
{
    private readonly IApiService _api;

    public AuthController(IApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _api.LoginAsync(model);

        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        HttpContext.Session.SetString("JwtToken",  result.Token);
        HttpContext.Session.SetString("Username",  result.Username);
        HttpContext.Session.SetString("Role",      result.Role);
        HttpContext.Session.SetString("TokenExpiry", result.Expiry.ToString("O"));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
