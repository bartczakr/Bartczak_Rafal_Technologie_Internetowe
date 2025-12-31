using Microsoft.EntityFrameworkCore;
using static Lab06_Bartczak_Rafal.DbModels;

namespace Lab06_Bartczak_Rafal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<NotesContext>(opts => opts.UseSqlite("Data Source=notes.db"));
            builder.Services.AddCors();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); 

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(); 

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<NotesContext>();
                db.Database.EnsureCreated();

                if (!db.Notes.Any())
                {
                    var tagWork = new Tag { Name = "work" };
                    var tagHome = new Tag { Name = "home" };
                    var tagIdeas = new Tag { Name = "ideas" };

                    var n1 = new Note { Title = "Witaj", Body = "To jest pierwsza notatka" };
                    n1.Tags.Add(tagWork); 

                    var n2 = new Note { Title = "Plan", Body = "Kupiæ mleko i chleb" };
                    n2.Tags.Add(tagHome);

                    db.Tags.AddRange(tagWork, tagHome, tagIdeas);
                    db.Notes.AddRange(n1, n2);
                    db.SaveChanges();
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.MapGet("/api/notes", async (NotesContext db, string? q) => {
                var query = db.Notes.Include(n => n.Tags).AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {

                    query = query.Where(n => n.Title.Contains(q) || n.Body.Contains(q));
                }

                var notes = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
                return Results.Ok(notes);
            });


            app.MapPost("/api/notes", async (NotesContext db, CreateNoteDto dto) => {
                var note = new Note { Title = dto.Title, Body = dto.Body };
                db.Notes.Add(note);
                await db.SaveChangesAsync();
                return Results.Created($"/api/notes/{note.Id}", note);
            });

            app.MapGet("/api/tags", async (NotesContext db) => await db.Tags.ToListAsync());

            app.MapPost("/api/notes/{id}/tags", async (NotesContext db, int id, AssignTagsDto dto) => {
                var note = await db.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == id);
                if (note == null) return Results.NotFound();

                note.Tags.Clear();

                foreach (var tagName in dto.Tags)
                {
                    var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                    }

                    note.Tags.Add(tag);
                }

                await db.SaveChangesAsync();
                return Results.Ok(note);
            });

            app.Run();
        }
    }
}
