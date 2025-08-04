
namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Quaries;
public class GetAllRolesQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class GetAllRolesQueryRequest : IRequest<IEnumerable<GetAllRolesQueryResponse>>
{
}

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQueryRequest, IEnumerable<GetAllRolesQueryResponse>>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    public GetAllRolesQueryHandler(RoleManager<AppRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GetAllRolesQueryResponse>> Handle(GetAllRolesQueryRequest request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
        return _mapper.Map<GetAllRolesQueryResponse, AppRole>(roles);
    }
}


