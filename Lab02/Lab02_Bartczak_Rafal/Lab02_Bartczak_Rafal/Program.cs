
using Microsoft.EntityFrameworkCore;

namespace Lab02_Bartczak_Rafal
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // bd
            builder.Services.AddDbContext<ShopContext>(opts => opts.UseSqlite("Data Source=shop.db"));
            builder.Services.AddCors();

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
                db.Database.EnsureCreated(); 

                if (!db.Products.Any())
                {
                    db.Products.AddRange(
                        new Product { Name = "Klawiatura", Price = 129.99m },
                        new Product { Name = "Mysz", Price = 79.90m },
                        new Product { Name = "Monitor", Price = 899.00m }
                    );
                    db.SaveChanges();
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // api

            // pobiera produkty
            app.MapGet("/api/products", async (ShopContext db) => await db.Products.ToListAsync());

            // dodaje produkty
            app.MapPost("/api/products", async (ShopContext db, Product p) => {
                if (p.Price < 0) return Results.BadRequest("Cena musi byæ >= 0");
                db.Products.Add(p);
                await db.SaveChangesAsync();
                return Results.Created($"/api/products/{p.Id}", p);
            });

            // pobiera koszyk
            app.MapGet("/api/cart", async (ShopContext db) => {
                var items = await db.CartItems.Include(c => c.Product).ToListAsync();
                var total = items.Sum(i => i.Qty * (i.Product?.Price ?? 0));
                return Results.Ok(new { items, total });
            });

            // dodaje do koszyka
            app.MapPost("/api/cart/add", async (ShopContext db, AddToCartRequest req) => {
                if (req.Qty <= 0) return Results.BadRequest("Iloœæ musi byæ > 0");

                var product = await db.Products.FindAsync(req.ProductId);
                if (product == null) return Results.NotFound();

                var existing = await db.CartItems.FirstOrDefaultAsync(c => c.ProductId == req.ProductId);
                if (existing != null) existing.Qty += req.Qty;
                else db.CartItems.Add(new CartItem { ProductId = req.ProductId, Qty = req.Qty });

                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // zmienia ilosc w koszyk
            app.MapPatch("/api/cart/item", async (ShopContext db, UpdateCartRequest req) => {
                if (req.Qty <= 0) return Results.BadRequest();
                var item = await db.CartItems.FirstOrDefaultAsync(c => c.ProductId == req.ProductId);
                if (item == null) return Results.NotFound();
                item.Qty = req.Qty;
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // usuwa z koszyka
            app.MapDelete("/api/cart/item/{productId}", async (ShopContext db, int productId) => {
                var item = await db.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId);
                if (item != null) { db.CartItems.Remove(item); await db.SaveChangesAsync(); }
                return Results.Ok();
            });

            // kupuje
            app.MapPost("/api/checkout", async (ShopContext db) => {
                var cartItems = await db.CartItems.Include(c => c.Product).ToListAsync();
                if (!cartItems.Any()) return Results.BadRequest("Pusty koszyk");

                using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    // tworzy zanmowienie
                    var order = new Order { CreatedAt = DateTime.Now };
                    db.Orders.Add(order);
                    await db.SaveChangesAsync();

                    decimal total = 0;

                    // przenosi produkty i zapisuje cene
                    foreach (var item in cartItems)
                    {
                        var currentPrice = item.Product!.Price;
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            Price = currentPrice 
                        };
                        db.OrderItems.Add(orderItem);
                        total += (currentPrice * item.Qty);
                    }

                    // czysci koszyk
                    db.CartItems.RemoveRange(cartItems);

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Results.Created($"/api/orders/{order.Id}", new { order_id = order.Id, total });
                }
                catch { await transaction.RollbackAsync(); return Results.StatusCode(500); }
            });

            // historia zamowienia
            app.MapGet("/api/orders", async (ShopContext db) => {
                var orders = await db.Orders
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var result = orders.Select(o => new {
                    o.Id,
                    o.CreatedAt,
                    Total = o.OrderItems.Sum(oi => oi.Qty * oi.Price),
                    Items = o.OrderItems.Select(oi => new {
                        ProductName = oi.Product?.Name ?? "Deleted",
                        oi.Qty,
                        oi.Price
                    })
                });
                return Results.Ok(result);
            });

            app.Run();
        }
    }
}
