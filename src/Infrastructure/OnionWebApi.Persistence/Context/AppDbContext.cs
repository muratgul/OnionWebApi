namespace OnionWebApi.Persistence.Context;
public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>, IAppDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
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
            if (typeof(ISoftDeletable).IsAssignableFrom(type))
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
            .Where(e => e.Entity is BaseAuditableEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        //activate this code before production
        var userId = "1";//_httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

        foreach (var entry in entries)
        {
            if (entry.Entity is ISoftDeletable && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                ((ISoftDeletable)entry.Entity).IsDeleted = true;
                ((ISoftDeletable)entry.Entity).DeletedUserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            }

            var entity = (BaseAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedUserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
                entity.CreatedDate = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedUserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
                entity.UpdatedDate = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
