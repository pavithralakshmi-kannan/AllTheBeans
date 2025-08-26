using Microsoft.EntityFrameworkCore;
using AllTheBeans.Models;

namespace AllTheBeans.Data;

public class BeansDb : DbContext
{
    public BeansDb(DbContextOptions<BeansDb> options) : base(options) { }

    public DbSet<Bean> Beans { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<BotdAssignment> BotdAssignments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bean>().HasIndex(b => b.Name);
        modelBuilder.Entity<BotdAssignment>().HasIndex(b => b.Date).IsUnique();
    }
}
