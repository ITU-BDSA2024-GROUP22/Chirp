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


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cheep>()
            .HasOne(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Bio>()
            .HasOne(b => b.Author)
            .WithOne(a => a.Bio)
            .HasForeignKey<Bio>(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
