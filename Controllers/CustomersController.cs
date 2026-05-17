using Microsoft.AspNetCore.Mvc;
using OrderFlowMVC.Models;
using OrderFlowMVC.Services;

namespace OrderFlowMVC.Controllers;

public class CustomersController : Controller
{
    private readonly IApiService _api;

    public CustomersController(IApiService api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
    private bool IsAdmin => HttpContext.Session.GetString("Role") == "Admin";

    public async Task<IActionResult> Index(string? search)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        ViewBag.Search = search;
        var customers = await _api.GetCustomersAsync(search);
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        var customer = await _api.GetCustomerAsync(id);
        if (customer is null) return NotFound();

        var orders = await _api.GetOrdersByCustomerAsync(id);
        ViewBag.Orders = orders;
        return View(customer);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        return View(new CreateCustomerRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerRequest model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        if (!ModelState.IsValid) return View(model);

        var result = await _api.CreateCustomerAsync(model);
        if (result is null)
        {
            TempData["Error"] = "Failed to create customer. Please try again.";
            return View(model);
        }

        TempData["Success"] = $"Customer {result.FullName} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var customer = await _api.GetCustomerAsync(id);
        if (customer is null) return NotFound();

        var model = new UpdateCustomerRequest
        {
            FirstName = customer.FirstName,
            LastName  = customer.LastName,
            Email     = customer.Email,
            Phone     = customer.Phone,
            Address   = customer.Address,
            IsActive  = customer.IsActive
        };
        ViewBag.CustomerId = id;
        ViewBag.FullName   = customer.FullName;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCustomerRequest model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();
        if (!ModelState.IsValid) { ViewBag.CustomerId = id; return View(model); }

        var result = await _api.UpdateCustomerAsync(id, model);
        if (result is null)
        {
            TempData["Error"] = "Failed to update customer.";
            ViewBag.CustomerId = id;
            return View(model);
        }

        TempData["Success"] = $"Customer {result.FullName} updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var success = await _api.DeleteCustomerAsync(id);
        TempData[success ? "Success" : "Error"] = success
            ? "Customer deleted successfully."
            : "Failed to delete customer.";
        return RedirectToAction(nameof(Index));
    }
}
