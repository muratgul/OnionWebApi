
namespace OnionWebApi.Persistence.Context;
public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
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

        foreach (var entry in entries)
        {
            var entity = (EntityBase)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
