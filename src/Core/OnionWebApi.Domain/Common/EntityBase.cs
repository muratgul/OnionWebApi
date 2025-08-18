namespace OnionWebApi.Domain.Common;

public class EntityBase : IEntityBase
{
    public int Id
    {
        get; set;
    }
    public DateTime CreatedDate
    {
        get; set;
    }
    public int CreatedUserId
    {
        get; set;
    }
    public DateTime? UpdatedDate { get; set; } = null;
    public int? UpdatedUserId
    {
        get; set;
    }
    public bool IsDeleted { get; set; } = false;
    public int? DeletedUserId
    {
        get; set;
    }
    public string CreatedUserName => GetCreatedUserName();
    public string? UpdatedUserName => GetUpdatedUserName();
    private string GetCreatedUserName()
    {
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        if (userManager == null)
        {
            return "";
        }

        if (CreatedUserId > 0)
        {
            var appUser = userManager.Users.First(p => p.Id == CreatedUserId);
            return appUser.FullName;
        }

        return "";
    }

    private string? GetUpdatedUserName()
    {
        if (UpdatedUserId is null)
        {
            return null;
        }

        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        if (userManager == null)
        {
            return null;
        }

        if (UpdatedUserId > 0)
        {
            var appUser = userManager.Users.First(p => p.Id == UpdatedUserId);
            return appUser.FullName;
        }

        return "";
    }
}
