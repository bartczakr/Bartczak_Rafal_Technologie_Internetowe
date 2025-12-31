
using Microsoft.EntityFrameworkCore;

namespace Lab01_Bartczak_Rafal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // baza danych w pliku library.db
            builder.Services.AddDbContext<LibraryContext>(opts =>
                opts.UseSqlite("Data Source=library.db"));

            builder.Services.AddCors();
            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            // automatyczne tworzenie bazy i danych
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                db.Database.EnsureCreated();
                if (!db.Books.Any())
                {
                    db.Members.AddRange(
                        new Member { Name = "Ala Kowalska", Email = "ala@example.com" },
                        new Member { Name = "Jan Nowak", Email = "jan@example.com" }
                    );

                    db.Books.AddRange(
                        new Book { Title = "Atomic Habits", Author = "James Clear", Copies = 3 },
                        new Book { Title = "Rich Dad Poor Dad", Author = "Robert Kiyosaki", Copies = 2 },
                        new Book { Title = "Can't Hurt Me", Author = "David Goggins", Copies = 4 }
                    );
                    db.SaveChanges();
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // api

            // czytelnicy
            app.MapGet("/api/members", async (LibraryContext db) => await db.Members.ToListAsync());

            app.MapPost("/api/members", async (LibraryContext db, Member m) => {
                try
                {
                    db.Members.Add(m);
                    await db.SaveChangesAsync();
                    return Results.Created($"/api/members/{m.Id}", m);
                }
                catch { return Results.Conflict("Email zajety"); }
            });

            // usuwanie czytelnika jakby cos sie zle dodalo ale to taka dev opcja
            app.MapDelete("/api/members/{id}", async (LibraryContext db, int id) => {
                var member = await db.Members.FindAsync(id);
                if (member == null) return Results.NotFound();

                db.Members.Remove(member);
                await db.SaveChangesAsync();
                return Results.Ok(new { message = "Usuniêto" });
            });

            // ksiazki
            app.MapGet("/api/books", async (LibraryContext db) => {
                var books = await db.Books.Select(b => new {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Copies,
                    Available = b.Copies - db.Loans.Count(l => l.BookId == b.Id && l.ReturnDate == null)
                }).ToListAsync();
                return Results.Ok(books);
            });

            app.MapPost("/api/books", async (LibraryContext db, Book b) => {
                db.Books.Add(b); await db.SaveChangesAsync(); return Results.Created($"/api/books/{b.Id}", b);
            });

            // wypozyczenia
            app.MapGet("/api/loans", async (LibraryContext db) =>
                await db.Loans.Include(l => l.Member).Include(l => l.Book).OrderByDescending(l => l.LoanDate)
                .Select(l => new { l.Id, MemberName = l.Member!.Name, BookTitle = l.Book!.Title, l.LoanDate, l.ReturnDate }).ToListAsync());

            app.MapPost("/api/loans/borrow", async (LibraryContext db, BorrowRequest req) => {
                var book = await db.Books.FindAsync(req.BookId);
                if (book == null) return Results.NotFound();

                var active = await db.Loans.CountAsync(l => l.BookId == req.BookId && l.ReturnDate == null);
                if (active >= book.Copies) return Results.Conflict("Brak kopii");

                var loan = new Loan { MemberId = req.MemberId, BookId = req.BookId, LoanDate = DateTime.Now, DueDate = DateTime.Now.AddDays(req.Days) };
                db.Loans.Add(loan); await db.SaveChangesAsync(); return Results.Created($"/api/loans/{loan.Id}", loan);
            });

            app.MapPost("/api/loans/return", async (LibraryContext db, int loanId) => {
                var loan = await db.Loans.FindAsync(loanId);
                if (loan == null || loan.ReturnDate != null) return Results.Conflict("Blad");
                loan.ReturnDate = DateTime.Now; await db.SaveChangesAsync(); return Results.Ok();
            });

            app.Run();
        }
    }
}