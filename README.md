# OrderFlow — ASP.NET Core MVC Dashboard

A premium, production-ready ASP.NET Core MVC web application that integrates with the Customer Ordering System API. Features a modern dark UI with full CRUD management for Customers, Products, and Orders.

---

## Screenshots / Features

- **Dashboard** — Live stats (customers, products, orders, revenue), recent orders, top products
- **Customers** — Search, create, view, edit, delete with order history per customer
- **Products** — Category filtering, low-stock alerts, card grid layout, full CRUD
- **Orders** — Status filtering, dynamic order builder with live total calculation, status management
- **Auth** — JWT-based login/logout with role-aware UI (Admin vs User)

---

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ (or 10.0.103 if available) |
| API Backend | Running on `http://localhost:4000` |

---

## Quick Start

### 1. Start the API backend first

Make sure the API project is running on `http://localhost:4000`. Refer to the API project's documentation for setup.

Default seeded credentials:
- **Admin** — username: `admin` / password: `Admin@123`
- **User** — username: `user1` / password: `User@123`

### 2. Configure the API URL (if different)

Edit `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:4000/"
  }
}
```

### 3. Restore & Run

```bash
cd OrderFlowMVC
dotnet restore
dotnet run
```

The MVC app will start on:
- **http://localhost:5000** (HTTP)
- **https://localhost:5001** (HTTPS)

Open your browser to `http://localhost:5000` — you'll be redirected to the login page.

---

## Project Structure

```
OrderFlowMVC/
├── Controllers/
│   ├── AuthController.cs        # Login / Logout
│   ├── HomeController.cs        # Dashboard
│   ├── CustomersController.cs   # Customer CRUD
│   ├── ProductsController.cs    # Product CRUD
│   └── OrdersController.cs      # Order management
│
├── Models/
│   └── ApiModels.cs             # DTOs matching API contracts
│
├── Services/
│   └── ApiService.cs            # HTTP client service layer
│
├── ViewModels/
│   ├── DashboardViewModel.cs
│   └── CreateOrderViewModel.cs
│
├── Views/
│   ├── Auth/Login.cshtml
│   ├── Home/Index.cshtml         # Dashboard
│   ├── Customers/               # Index, Details, Create, Edit
│   ├── Products/                # Index, Details, Create, Edit
│   ├── Orders/                  # Index, Details, Create
│   └── Shared/_Layout.cshtml    # App shell with sidebar
│
├── wwwroot/
│   └── css/site.css             # Full dark theme design system
│
├── appsettings.json
├── Program.cs
└── NuGet.Config                 # Configured for offline/local restore
```

---

## API Endpoints Used

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | JWT login |
| GET | `/api/customers` | List / search customers |
| GET/POST/PUT/DELETE | `/api/customers/{id}` | Customer CRUD |
| GET | `/api/products` | List products |
| GET/POST/PUT/DELETE | `/api/products/{id}` | Product CRUD |
| GET | `/api/orders` | List orders |
| GET/POST/DELETE | `/api/orders/{id}` | Order management |
| GET | `/api/orders/customer/{id}` | Orders by customer |
| PATCH | `/api/orders/{id}/status` | Update order status |

---

## Role-Based Access

| Feature | Admin | User |
|---------|-------|------|
| View all data | ✅ | ✅ |
| Create customers | ✅ | ❌ |
| Edit customers | ✅ | ❌ |
| Delete customers | ✅ | ❌ |
| Create products | ✅ | ❌ |
| Edit products | ✅ | ❌ |
| Delete products | ✅ | ❌ |
| Create orders | ✅ | ✅ |
| Update order status | ✅ | ❌ |
| Delete orders | ✅ | ❌ |

---

## Design System

- **Font**: DM Sans (body) + Syne (headings/brand)
- **Theme**: Dark — `#0c0e12` background, `#131519` surfaces
- **Accent**: `#6c63ff` purple + `#00d4aa` teal
- **Responsive**: Collapsible sidebar on mobile/tablet
- **Animations**: Toast notifications, hover transitions, pulse indicators

---

## Troubleshooting

**"Connection refused" errors on login:**
→ The API backend is not running. Start the API on port 4000.

**"Invalid username or password":**
→ Ensure the API database is seeded. The API seeds on first run.

**NuGet restore fails offline:**
→ The included `NuGet.Config` uses local SDK packages. If online, delete `NuGet.Config` to use nuget.org.

**HTTPS certificate error:**
→ Run `dotnet dev-certs https --trust` once, or use `http://localhost:5000`.
