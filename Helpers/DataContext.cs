using Microsoft.EntityFrameworkCore;
using aspnetapp.Entities;

namespace aspnetapp.Helpers
{
    public class DataContext : DbContext
    {
    protected readonly IConfiguration Configuration;

    //public static DataContext? Inited { get; private set; } 

    public DataContext(DbContextOptions<DataContext> options):base(options) {
        //DataContext.Inited = this;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.User>().HasMany(u => u.Tokens).WithOne(t => t.User).HasForeignKey(t => t.UserEmail).IsRequired();
        modelBuilder.Entity<Entities.User>().HasMany(u => u.Actions).WithOne(a => a.User).HasForeignKey(a => a.UserEmail).IsRequired();
        modelBuilder.Entity<Entities.Action>().HasKey(ah => new { ah.UserEmail, ah.Date});
    }

    public DbSet<Entities.User> Users { get; set; }
    public DbSet<Entities.Token> Tokens { get; set; }
    public DbSet<Entities.Action> Actions { get; set; }
    }
}