using Microsoft.EntityFrameworkCore;
using ModelDescriptionsApi.Models;

namespace ModelDescriptionsApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Asset3d> asset3ds { get; set; } = null!;
}
