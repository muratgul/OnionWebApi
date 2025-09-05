namespace OnionWebApi.Application.Bases;
public class BaseHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService)
{
    public readonly IMapper _mapper = mapper;
    public readonly IUnitOfWork _unitOfWork = unitOfWork;
    public readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public readonly string _userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    public readonly IUriService _uriService = uriService;
    public readonly ICacheService _cacheService = cacheService;
}