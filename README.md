# sample-backend-api

A small ASP.NET Core Web API (Orders domain) that serves as a **safe test subject** for the AI Development Orchestrator. It is not a product.

## Endpoints
| Method | Route | Description |
|---|---|---|
| GET | `/api/customers` | All customers |
| GET | `/api/customers/{id}` | One customer |
| GET | `/api/customers/{id}/orders` | All orders of a customer, newest first |
| GET | `/api/products` | All products |
| GET | `/api/orders/{id}` | One order with items |
| POST | `/api/orders` | Create an order (validates customer, products, stock) |

## Run
```powershell
dotnet run --project src/Orders.Api      # uses (localdb)\MSSQLLocalDB, creates + seeds SampleOrdersDb
dotnet test
```

See [AGENTS.md](AGENTS.md) for architecture and conventions.
