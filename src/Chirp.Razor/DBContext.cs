using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class DBContext : DbContext
{
    public DbSet<Cheep> messages { get; set; }
    public DbSet<Author> users { get; set; }

    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {

    }

}
