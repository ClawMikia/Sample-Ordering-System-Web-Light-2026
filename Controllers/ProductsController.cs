using Microsoft.AspNetCore.Mvc;
using OrderFlowMVC.Models;
using OrderFlowMVC.Services;

namespace OrderFlowMVC.Controllers;

public class ProductsController : Controller
{
    private readonly IApiService _api;

    public ProductsController(IApiService api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
    private bool IsAdmin => HttpContext.Session.GetString("Role") == "Admin";

    public async Task<IActionResult> Index(string? category, bool? lowStock)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        var products = await _api.GetProductsAsync();

        if (!string.IsNullOrEmpty(category))
            products = products.Where(p => p.Category == category).ToList();

        if (lowStock == true)
            products = products.Where(p => p.StockQuantity < 10).ToList();

        ViewBag.Categories = products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        ViewBag.SelectedCategory = category;
        ViewBag.LowStock = lowStock;
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        var product = await _api.GetProductAsync(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        return View(new CreateProductRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductRequest model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        if (!ModelState.IsValid) return View(model);

        var result = await _api.CreateProductAsync(model);
        if (result is null)
        {
            TempData["Error"] = "Failed to create product.";
            return View(model);
        }

        TempData["Success"] = $"Product '{result.Name}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var product = await _api.GetProductAsync(id);
        if (product is null) return NotFound();

        var model = new UpdateProductRequest
        {
            Name          = product.Name,
            Description   = product.Description,
            Price         = product.Price,
            StockQuantity = product.StockQuantity,
            Category      = product.Category,
            IsActive      = product.IsActive
        };
        ViewBag.ProductId   = id;
        ViewBag.ProductName = product.Name;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductRequest model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        if (!ModelState.IsValid) { ViewBag.ProductId = id; return View(model); }

        var result = await _api.UpdateProductAsync(id, model);
        if (result is null)
        {
            TempData["Error"] = "Failed to update product.";
            ViewBag.ProductId = id;
            return View(model);
        }

        TempData["Success"] = $"Product '{result.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var success = await _api.DeleteProductAsync(id);
        TempData[success ? "Success" : "Error"] = success
            ? "Product deleted successfully."
            : "Failed to delete product.";
        return RedirectToAction(nameof(Index));
    }
}
