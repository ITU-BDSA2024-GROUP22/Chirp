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

    public DbSet<Follow> Follows { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Follow>()
            .HasKey(f => new { f.FollowerUserId, f.AuthorUserId });

    builder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(a => a.FollowingList)
            .HasForeignKey(f => f.FollowerUserId);

        builder.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(a => a.FollowersList)
            .HasForeignKey(f => f.AuthorUserId);
    }
    public override void Dispose()
    {
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }
}
