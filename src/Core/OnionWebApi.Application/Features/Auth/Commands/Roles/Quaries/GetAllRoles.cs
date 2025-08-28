namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Quaries;
public class GetAllRolesQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

public class GetAllRolesQueryRequest : IRequest<IEnumerable<GetAllRolesQueryResponse>>
{
}

public class GetAllRolesQueryHandler(RoleManager<AppRole> roleManager, IMapper mapper) : IRequestHandler<GetAllRolesQueryRequest, IEnumerable<GetAllRolesQueryResponse>>
{
    private readonly RoleManager<AppRole> _roleManager = roleManager;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<GetAllRolesQueryResponse>> Handle(GetAllRolesQueryRequest request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
        return _mapper.Map<List<GetAllRolesQueryResponse>>(roles);
    }
}


