using Microsoft.EntityFrameworkCore;

namespace Lab01_Bartczak_Rafal
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Member> Members => Set<Member>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().ToTable("Members");
            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Loan>().ToTable("Loans");
            modelBuilder.Entity<Member>().HasIndex(m => m.Email).IsUnique();
        }
    }
}
