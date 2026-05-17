namespace OrderFlowMVC.Models;

// ── Auth ─────────────────────────────────────────────────────
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token    { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role     { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}

// ── Customer ─────────────────────────────────────────────────
public class CustomerDto
{
    public int      Id        { get; set; }
    public string   FirstName { get; set; } = string.Empty;
    public string   LastName  { get; set; } = string.Empty;
    public string   Email     { get; set; } = string.Empty;
    public string   Phone     { get; set; } = string.Empty;
    public string   Address   { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool     IsActive  { get; set; }
    public string   FullName  => $"{FirstName} {LastName}";
}

public class CreateCustomerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Phone     { get; set; } = string.Empty;
    public string Address   { get; set; } = string.Empty;
}

public class UpdateCustomerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Phone     { get; set; } = string.Empty;
    public string Address   { get; set; } = string.Empty;
    public bool   IsActive  { get; set; }
}

// ── Product ──────────────────────────────────────────────────
public class ProductDto
{
    public int     Id            { get; set; }
    public string  Name          { get; set; } = string.Empty;
    public string  Description   { get; set; } = string.Empty;
    public decimal Price         { get; set; }
    public int     StockQuantity { get; set; }
    public string  Category      { get; set; } = string.Empty;
    public bool    IsActive      { get; set; }
}

public class CreateProductRequest
{
    public string  Name          { get; set; } = string.Empty;
    public string  Description   { get; set; } = string.Empty;
    public decimal Price         { get; set; }
    public int     StockQuantity { get; set; }
    public string  Category      { get; set; } = string.Empty;
}

public class UpdateProductRequest
{
    public string  Name          { get; set; } = string.Empty;
    public string  Description   { get; set; } = string.Empty;
    public decimal Price         { get; set; }
    public int     StockQuantity { get; set; }
    public string  Category      { get; set; } = string.Empty;
    public bool    IsActive      { get; set; }
}

// ── Order ────────────────────────────────────────────────────
public class OrderItemDto
{
    public int     Id          { get; set; }
    public int     ProductId   { get; set; }
    public string  ProductName { get; set; } = string.Empty;
    public int     Quantity    { get; set; }
    public decimal UnitPrice   { get; set; }
    public decimal Subtotal    { get; set; }
}

public class OrderDto
{
    public int              Id           { get; set; }
    public int              CustomerId   { get; set; }
    public string           CustomerName { get; set; } = string.Empty;
    public DateTime         OrderDate    { get; set; }
    public string           Status       { get; set; } = string.Empty;
    public decimal          TotalAmount  { get; set; }
    public string           Notes        { get; set; } = string.Empty;
    public List<OrderItemDto> Items      { get; set; } = new();
}

public class CreateOrderRequest
{
    public int                       CustomerId { get; set; }
    public string                    Notes      { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items   { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity  { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

// ── Common ───────────────────────────────────────────────────
public class ApiResponse<T>
{
    public bool    Success { get; set; }
    public string  Message { get; set; } = string.Empty;
    public T?      Data    { get; set; }
}
