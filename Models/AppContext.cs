using Microsoft.EntityFrameworkCore;

namespace SalesDB_EF.Models;

public class AppContext : DbContext
{
    private readonly string _connectionString;
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }

    public AppContext(string connectionString)
    {
        _connectionString = connectionString;
        
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}