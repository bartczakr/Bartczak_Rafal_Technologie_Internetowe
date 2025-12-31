using Microsoft.EntityFrameworkCore;

namespace Lab05_Bartczak_Rafal
{
    public class KanbanContext : DbContext
    {
        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        public DbSet<Column> Columns => Set<Column>();
        public DbSet<KanbanTask> Tasks => Set<KanbanTask>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Column>().ToTable("Columns");
            modelBuilder.Entity<KanbanTask>().ToTable("Tasks");

            modelBuilder.Entity<KanbanTask>()
                .HasOne(t => t.Column)
                .WithMany()
                .HasForeignKey(t => t.ColId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
