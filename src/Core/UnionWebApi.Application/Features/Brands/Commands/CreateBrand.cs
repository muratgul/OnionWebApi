namespace UnionWebApi.Application.Features.Brands.Commands;
public class CreateBrandCommandRequest : IRequest<Unit>
{
    public string Name { get; set; }
}

public class CreateBrandCommandHandler : BaseHandler, IRequestHandler<CreateBrandCommandRequest, Unit>
{
    public CreateBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
    {
    }

    public async Task<Unit> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {        
        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(new Brand
        {
            Name = request.Name,
            IsDeleted = false
        });

        await _unitOfWork.SaveAsync();

        return Unit.Value;
    }
}
