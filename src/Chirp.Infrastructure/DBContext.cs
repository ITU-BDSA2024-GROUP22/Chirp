using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class DBContext : IdentityDbContext<Author>
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
    }

    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DbSet<Bio> Bios { get; set; }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }
}
