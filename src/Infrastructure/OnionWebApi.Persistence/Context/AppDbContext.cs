namespace OnionWebApi.Persistence.Context;
public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>, IAppDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly TimeProvider _timeProvider;
    public AppDbContext()
    {
        _timeProvider = TimeProvider.System;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor, TimeProvider? timeProvider) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplySoftDeleteQueryFilter(modelBuilder);
    }
    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                // Global query filter: IsDeleted == false
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyAccess = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Equal(propertyAccess, Expression.Constant(false));
                var lambda = Expression.Lambda(filter, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }
    public override int SaveChanges()
    {
        HandleAuditableEntities();
        return base.SaveChanges();
    }
    private void HandleAuditableEntities()
    {
        var userId = GetCurrentUserId();
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseAuditableEntity &&
                        e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            // Soft Delete
            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedUserId = userId;
                softDeletable.DeletedDate = now;
            }

            // Audit fields
            if (entry.Entity is BaseAuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedUserId = userId;
                        auditable.CreatedDate = now;
                        break;

                    case EntityState.Modified:
                        // CreatedDate ve CreatedUserId değişmesin
                        entry.Property(nameof(BaseAuditableEntity.CreatedDate)).IsModified = false;
                        entry.Property(nameof(BaseAuditableEntity.CreatedUserId)).IsModified = false;

                        auditable.UpdatedUserId = userId;
                        auditable.UpdatedDate = now;
                        break;
                }
            }
        }
    }
    private int GetCurrentUserId()
    {
        if (_httpContextAccessor?.HttpContext == null)
        {
            return 0;
        }

        var userIdClaim = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
