using System.Collections.Generic;
using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Auth.Queries;
public class GetAllUsersQueryResponse : UserDto
{
    //public IList<string> Roles { get; set; }
}

public class GetAllUsersQueryRequest : IRequest<IEnumerable<GetAllUsersQueryResponse>>
{
}

public class GetAllUsersQueryHandler : BaseHandler, IRequestHandler<GetAllUsersQueryRequest, IEnumerable<GetAllUsersQueryResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    public GetAllUsersQueryHandler(UserManager<AppUser> userManager,  IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<GetAllUsersQueryResponse>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();
        
        //List with roles
        /*
        var list = new List<GetAllUsersQueryResponse>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            list.Add(new GetAllUsersQueryResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                PasswordHash = user.PasswordHash,
                UserName = user.UserName,
                Roles = roles.ToList()
            });
        }
        */

        var mappedUsers = _mapper.Map<List<GetAllUsersQueryResponse>>(users);

        return mappedUsers;
    }
} 
