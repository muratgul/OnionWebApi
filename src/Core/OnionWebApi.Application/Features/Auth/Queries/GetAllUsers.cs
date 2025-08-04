namespace OnionWebApi.Application.Features.Auth.Queries;
public class GetAllUsersQueryResponse : UserDto
{
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
        var users = await _userManager.Users.ToListAsync();

        var mappedUsers = _mapper.Map<GetAllUsersQueryResponse, AppUser>(users);

        return mappedUsers;
    }
} 
