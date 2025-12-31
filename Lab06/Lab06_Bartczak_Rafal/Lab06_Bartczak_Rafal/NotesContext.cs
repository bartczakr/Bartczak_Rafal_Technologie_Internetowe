using Microsoft.EntityFrameworkCore;
using static Lab06_Bartczak_Rafal.DbModels;

namespace Lab06_Bartczak_Rafal
{
    public class NotesContext : DbContext
    {
        public NotesContext(DbContextOptions<NotesContext> options) : base(options) { }

        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Tag> Tags => Set<Tag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>().ToTable("Notes");
            modelBuilder.Entity<Tag>().ToTable("Tags");

            modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();


            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags)
                .WithMany(t => t.Notes)
                .UsingEntity(j => j.ToTable("NoteTags"));
        }
    }
}
