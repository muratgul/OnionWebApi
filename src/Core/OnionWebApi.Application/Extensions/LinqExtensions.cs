namespace OnionWebApi.Application.Extensions;
public static class LinqExtensions
{
    /// <summary>
    /// Birden fazla navigation property'yi tek seferde include eder
    /// </summary>
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[]? includes) where T : class
    {
        if (includes?.Length > 0)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }
        return query;
    }

    /// <summary>
    /// String property name kullanarak dinamik sıralama yapar
    /// </summary>
    public static IOrderedQueryable<TSource> OrderByDynamic<TSource>(this IQueryable<TSource> query, string propertyName, ESort sortDirection = ESort.ASC)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Property name cannot be null or empty", nameof(propertyName));
        }

        var entityType = typeof(TSource);
        var propertyInfo = GetPropertyInfo(entityType, propertyName) ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{entityType.Name}'", nameof(propertyName));

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection == ESort.ASC ? "OrderBy" : "OrderByDescending";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType, propertyInfo.PropertyType);

        var result = method.Invoke(null, [query, lambda]) as IOrderedQueryable<TSource>;
        return result ?? throw new InvalidOperationException("Failed to create ordered query");
    }

    /// <summary>
    /// Zaten sıralanmış sorguya ek sıralama kriterleri ekler
    /// </summary>
    public static IOrderedQueryable<TSource> ThenByDynamic<TSource>(this IOrderedQueryable<TSource> query, string propertyName, ESort sortDirection = ESort.ASC)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Property name cannot be null or empty", nameof(propertyName));
        }

        var entityType = typeof(TSource);
        var propertyInfo = GetPropertyInfo(entityType, propertyName) ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{entityType.Name}'", nameof(propertyName));

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection == ESort.ASC ? "ThenBy" : "ThenByDescending";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType, propertyInfo.PropertyType);

        var result = method.Invoke(null, [query, lambda]) as IOrderedQueryable<TSource>;
        return result ?? throw new InvalidOperationException("Failed to create ordered query");
    }

    /// <summary>
    /// String tabanlı conditional include için
    /// </summary>
    public static IQueryable<T> IncludeIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, object>> include) where T : class
    {
        return condition ? query.Include(include) : query;
    }

    /// <summary>
    /// Pagination desteği
    /// </summary>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Dynamic filtering desteği
    /// </summary>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// String contains araması (case insensitive)
    /// </summary>
    public static IQueryable<T> SearchByProperty<T>(this IQueryable<T> query, string propertyName, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || string.IsNullOrWhiteSpace(propertyName))
        {
            return query;
        }

        var entityType = typeof(T);
        var propertyInfo = GetPropertyInfo(entityType, propertyName);

        if (propertyInfo?.PropertyType != typeof(string))
        {
            return query;
        }

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyInfo);

        // EF Core için ToLower() kullanımı
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var propertyToLower = Expression.Call(property, toLowerMethod!);

        var searchValue = Expression.Constant(searchTerm.ToLower());
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
        var containsCall = Expression.Call(propertyToLower, containsMethod!, searchValue);

        var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

        return query.Where(lambda);
    }

    /// <summary>
    /// Multiple property'lerde arama
    /// </summary>
    public static IQueryable<T> SearchInMultipleProperties<T>(this IQueryable<T> query, string? searchTerm, params string[] propertyNames)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertyNames.Length == 0)
        {
            return query;
        }

        var entityType = typeof(T);
        var parameter = Expression.Parameter(entityType, "x");
        Expression? combinedExpression = null;

        foreach (var propertyName in propertyNames)
        {
            var propertyInfo = GetPropertyInfo(entityType, propertyName);
            if (propertyInfo?.PropertyType != typeof(string))
            {
                continue;
            }

            var property = Expression.Property(parameter, propertyInfo);
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var propertyToLower = Expression.Call(property, toLowerMethod!);

            var searchValue = Expression.Constant(searchTerm.ToLower());
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
            var containsCall = Expression.Call(propertyToLower, containsMethod!, searchValue);

            combinedExpression = combinedExpression == null
                ? containsCall
                : Expression.OrElse(combinedExpression, containsCall);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Property bilgisini case insensitive olarak getirir
    /// </summary>
    private static PropertyInfo? GetPropertyInfo(Type entityType, string propertyName)
    {
        return entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
    }
}

/*
// KULLANIM ÖRNEKLERİ

** Multiple includes
        var users = context.Users
            .IncludeMultiple(u => u.Profile, u => u.Orders, u => u.Roles)
            .ToList();

** Dynamic sorting
        var sortedUsers = context.Users
            .OrderByDynamic("Name", ESort.ASC)
            .ThenByDynamic("CreatedDate", ESort.DESC)
            .ToList();

** Conditional include
        var usersWithProfile = context.Users
            .IncludeIf(includeProfile, u => u.Profile)
            .ToList();

** Pagination
        var pagedUsers = context.Users
            .OrderByDynamic("Name")
            .Paginate(page: 2, pageSize: 10)
            .ToList();

** Conditional filtering
        var filteredUsers = context.Users
            .WhereIf(!string.IsNullOrEmpty(status), u => u.Status == status)
            .ToList();

** Search by property
        var searchResults = context.Users
            .SearchByProperty("Name", "john")
            .ToList();

** Multi-property search
        var multiSearchResults = context.Users
            .SearchInMultipleProperties("search term", "Name", "Email", "Description")
            .ToList();
 */