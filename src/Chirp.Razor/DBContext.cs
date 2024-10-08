using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class DBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {

    }



}
