namespace UnionWebApi.Application.Bases;
public class BaseHandler
{
    public readonly IMapper _mapper;
    public readonly IUnitOfWork _unitOfWork;
    public readonly IHttpContextAccessor _httpContextAccessor;
    public readonly string _userId;

    public BaseHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
