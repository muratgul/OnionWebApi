
using Microsoft.AspNetCore.Http;
using OnionWebApi.Application.Interfaces.DbContext;
using System.Security.Claims;

namespace OnionWebApi.Persistence.Context;
public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>, IAppDbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var type = entityType.ClrType;

            // BaseEntity'den türeyen ve IsDeleted property'si olan entity'ler için
            if (typeof(EntityBase).IsAssignableFrom(type) &&
                type.GetProperty("IsDeleted") != null)
            {
                var parameter = Expression.Parameter(type, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, "IsDeleted"),
                    Expression.Constant(false));

                modelBuilder.Entity(type)
                    .HasQueryFilter(Expression.Lambda(body, parameter));
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is EntityBase && 
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        HttpContextAccessor httpContextAccessor = new();
        string userIdString = httpContextAccessor.HttpContext!.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        var userId = int.Parse(userIdString);

        foreach (var entry in entries)
        {
            var entity = (EntityBase)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedUserId = userId;
                entity.CreatedDate = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedUserId = userId;
                entity.UpdatedDate = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
