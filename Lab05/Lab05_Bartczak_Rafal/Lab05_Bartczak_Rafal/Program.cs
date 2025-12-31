
using Microsoft.EntityFrameworkCore;

namespace Lab05_Bartczak_Rafal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // baza
            builder.Services.AddDbContext<KanbanContext>(opts => opts.UseSqlite("Data Source=kanban.db"));
            builder.Services.AddCors();

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();


            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<KanbanContext>();
                db.Database.EnsureCreated();

                if (!db.Columns.Any())
                {
                    db.Columns.AddRange(
                        new Column { Name = "Todo", Ord = 1 },
                        new Column { Name = "Doing", Ord = 2 },
                        new Column { Name = "Done", Ord = 3 }
                    );
                    db.SaveChanges();

                    db.Tasks.AddRange(
                        new KanbanTask { Title = "Setup project", ColId = 1, Ord = 1 },
                        new KanbanTask { Title = "Write docs", ColId = 1, Ord = 2 },
                        new KanbanTask { Title = "Release", ColId = 3, Ord = 1 }
                    );
                    db.SaveChanges();
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // api

            // pobierz tablice
            app.MapGet("/api/board", async (KanbanContext db) => {
                var cols = await db.Columns.OrderBy(c => c.Ord).ToListAsync();
                var tasks = await db.Tasks.OrderBy(t => t.Ord).ToListAsync();
                return Results.Ok(new { cols, tasks });
            });

            // dodaje zad
            app.MapPost("/api/tasks", async (KanbanContext db, CreateTaskDto dto) => {
                var maxOrd = await db.Tasks
                    .Where(t => t.ColId == dto.ColId)
                    .MaxAsync(t => (int?)t.Ord) ?? 0;

                var newTask = new KanbanTask
                {
                    Title = dto.Title,
                    ColId = dto.ColId,
                    Ord = maxOrd + 1 
                };

                db.Tasks.Add(newTask);
                await db.SaveChangesAsync();
                return Results.Created($"/api/tasks/{newTask.Id}", newTask);
            });

            // przenies zadanie
            app.MapPost("/api/tasks/{id}/move", async (KanbanContext db, int id, MoveTaskDto dto) => {
                var task = await db.Tasks.FindAsync(id);
                if (task == null) return Results.NotFound();

                if (task.ColId != dto.ColId)
                {
                    var newMaxOrd = await db.Tasks
                        .Where(t => t.ColId == dto.ColId)
                        .MaxAsync(t => (int?)t.Ord) ?? 0;

                    task.ColId = dto.ColId;
                    task.Ord = newMaxOrd + 1; 

                    await db.SaveChangesAsync();
                }

                return Results.Ok(task);
            });

            app.Run();
        }
    }
}
