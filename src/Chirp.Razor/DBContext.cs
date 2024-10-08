using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public static DbSet<Cheep> messages { get; set; }
    public static DbSet<Author> users { get; set; }

}
