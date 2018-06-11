using Microsoft.EntityFrameworkCore;

namespace PtcApi.Model
{
  public class PtcDbContext : DbContext
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<AppUserClaim> AppUserClaim { get; set; }

    private const string CONN =
                  @"Server=W7L5CG6196TT7\DXH028;
                    Database=Auth;
                    Trusted_Connection=True;
                    MultipleActiveResultSets=true";

    // private const string CONN = @"Server=(localdb)\MSSQLLocalDB;
    // Database=PTC-Pluralsight;
    // AttachDbFilename=D:\Samples\PTC\SqlData\PTC-Pluralsight.mdf;
    // MultipleActiveResultSets=true";

    protected override void OnConfiguring(
                DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(CONN);
    }
  }
}
