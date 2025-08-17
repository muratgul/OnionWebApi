namespace OnionWebApi.Domain.Common;

public class EntityBase : IEntityBase
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedUserId { get; set; }
    public string CreatedUserName => GetCreatedUserName();    
    public DateTime? UpdatedDate { get; set; } = null;
    public int? UpdatedUserId { get; set; }
    public string? UpdatedUserName => GetUpdatedUserName();
    public bool IsDeleted { get; set; } = false;
    public int? DeletedUserId { get; set; }
    private string GetCreatedUserName()
    {
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        var appUser = userManager.Users.First(p => p.Id == CreatedUserId);

        return $"{appUser.FullName}";
    }
    private string GetUpdatedUserName()
    {
        HttpContextAccessor httpContextAccessor = new();
        var userManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

        var appUser = userManager.Users.First(p => p.Id == UpdatedUserId);

        return $"{appUser.FullName}";
    }


}
