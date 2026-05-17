using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OrderFlowMVC.Models;

namespace OrderFlowMVC.Services;

public interface IApiService
{
    // Auth
    Task<LoginResponse?> LoginAsync(LoginRequest request);

    // Customers
    Task<List<CustomerDto>> GetCustomersAsync(string? search = null);
    Task<CustomerDto?> GetCustomerAsync(int id);
    Task<CustomerDto?> CreateCustomerAsync(CreateCustomerRequest request);
    Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerRequest request);
    Task<bool> DeleteCustomerAsync(int id);

    // Products
    Task<List<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductAsync(int id);
    Task<ProductDto?> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);

    // Orders
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderAsync(int id);
    Task<List<OrderDto>> GetOrdersByCustomerAsync(int customerId);
    Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, string status);
    Task<bool> DeleteOrderAsync(int id);
}

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;
    private readonly ILogger<ApiService> _logger;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient http, IHttpContextAccessor context, ILogger<ApiService> logger)
    {
        _http    = http;
        _context = context;
        _logger  = logger;
    }

    private void AttachToken()
    {
        var token = _context.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private StringContent Json(object obj) =>
        new(JsonSerializer.Serialize(obj, _json), Encoding.UTF8, "application/json");

    private async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return default;
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<T>>(body, _json);
        return result is { Success: true } ? result.Data : default;
    }

    // ── Auth ──────────────────────────────────────────────────
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsync("api/auth/login", Json(request));
            return await ReadAsync<LoginResponse>(response);
        }
        catch (Exception ex) { _logger.LogError(ex, "Login failed"); return null; }
    }

    // ── Customers ─────────────────────────────────────────────
    public async Task<List<CustomerDto>> GetCustomersAsync(string? search = null)
    {
        try
        {
            AttachToken();
            var url = string.IsNullOrEmpty(search) ? "api/customers" : $"api/customers?search={Uri.EscapeDataString(search)}";
            var response = await _http.GetAsync(url);
            return await ReadAsync<List<CustomerDto>>(response) ?? new();
        }
        catch (Exception ex) { _logger.LogError(ex, "GetCustomers failed"); return new(); }
    }

    public async Task<CustomerDto?> GetCustomerAsync(int id)
    {
        try { AttachToken(); return await ReadAsync<CustomerDto>(await _http.GetAsync($"api/customers/{id}")); }
        catch (Exception ex) { _logger.LogError(ex, "GetCustomer {Id} failed", id); return null; }
    }

    public async Task<CustomerDto?> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try { AttachToken(); return await ReadAsync<CustomerDto>(await _http.PostAsync("api/customers", Json(request))); }
        catch (Exception ex) { _logger.LogError(ex, "CreateCustomer failed"); return null; }
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
    {
        try { AttachToken(); return await ReadAsync<CustomerDto>(await _http.PutAsync($"api/customers/{id}", Json(request))); }
        catch (Exception ex) { _logger.LogError(ex, "UpdateCustomer {Id} failed", id); return null; }
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        try { AttachToken(); var r = await _http.DeleteAsync($"api/customers/{id}"); return r.IsSuccessStatusCode; }
        catch (Exception ex) { _logger.LogError(ex, "DeleteCustomer {Id} failed", id); return false; }
    }

    // ── Products ──────────────────────────────────────────────
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        try { AttachToken(); return await ReadAsync<List<ProductDto>>(await _http.GetAsync("api/products")) ?? new(); }
        catch (Exception ex) { _logger.LogError(ex, "GetProducts failed"); return new(); }
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        try { AttachToken(); return await ReadAsync<ProductDto>(await _http.GetAsync($"api/products/{id}")); }
        catch (Exception ex) { _logger.LogError(ex, "GetProduct {Id} failed", id); return null; }
    }

    public async Task<ProductDto?> CreateProductAsync(CreateProductRequest request)
    {
        try { AttachToken(); return await ReadAsync<ProductDto>(await _http.PostAsync("api/products", Json(request))); }
        catch (Exception ex) { _logger.LogError(ex, "CreateProduct failed"); return null; }
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try { AttachToken(); return await ReadAsync<ProductDto>(await _http.PutAsync($"api/products/{id}", Json(request))); }
        catch (Exception ex) { _logger.LogError(ex, "UpdateProduct {Id} failed", id); return null; }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try { AttachToken(); var r = await _http.DeleteAsync($"api/products/{id}"); return r.IsSuccessStatusCode; }
        catch (Exception ex) { _logger.LogError(ex, "DeleteProduct {Id} failed", id); return false; }
    }

    // ── Orders ────────────────────────────────────────────────
    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        try { AttachToken(); return await ReadAsync<List<OrderDto>>(await _http.GetAsync("api/orders")) ?? new(); }
        catch (Exception ex) { _logger.LogError(ex, "GetOrders failed"); return new(); }
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        try { AttachToken(); return await ReadAsync<OrderDto>(await _http.GetAsync($"api/orders/{id}")); }
        catch (Exception ex) { _logger.LogError(ex, "GetOrder {Id} failed", id); return null; }
    }

    public async Task<List<OrderDto>> GetOrdersByCustomerAsync(int customerId)
    {
        try { AttachToken(); return await ReadAsync<List<OrderDto>>(await _http.GetAsync($"api/orders/customer/{customerId}")) ?? new(); }
        catch (Exception ex) { _logger.LogError(ex, "GetOrdersByCustomer {Id} failed", customerId); return new(); }
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
    {
        try { AttachToken(); return await ReadAsync<OrderDto>(await _http.PostAsync("api/orders", Json(request))); }
        catch (Exception ex) { _logger.LogError(ex, "CreateOrder failed"); return null; }
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, string status)
    {
        try { AttachToken(); return await ReadAsync<OrderDto>(await _http.PatchAsync($"api/orders/{id}/status", Json(new UpdateOrderStatusRequest { Status = status }))); }
        catch (Exception ex) { _logger.LogError(ex, "UpdateOrderStatus {Id} failed", id); return null; }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try { AttachToken(); var r = await _http.DeleteAsync($"api/orders/{id}"); return r.IsSuccessStatusCode; }
        catch (Exception ex) { _logger.LogError(ex, "DeleteOrder {Id} failed", id); return false; }
    }
}
