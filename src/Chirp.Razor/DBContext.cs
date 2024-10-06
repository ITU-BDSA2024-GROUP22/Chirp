using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    DbSet<Cheep> messages { get; set; }
    DbSet<Author> users { get; set; }




}
