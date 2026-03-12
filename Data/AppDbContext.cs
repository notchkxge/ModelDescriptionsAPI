using ModelDescriptionsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ModelDescriptionsApi.Data;

public class ApplicationDbContext : DbContext{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}

  public DbSet<Description> descriptions {get;set;} = null!;
}
