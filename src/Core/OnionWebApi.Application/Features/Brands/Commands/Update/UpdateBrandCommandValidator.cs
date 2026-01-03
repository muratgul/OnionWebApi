
namespace OnionWebApi.Application.Features.Brands.Commands.Update;
public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommandRequest>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateBrandCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Brand ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name must not exceed 100 characters.")
            .MustAsync(async (model, name, cancellation) => await BeUniqueName(model.Id, name, cancellation))
            .WithMessage("Brand name already exists.");
        
    }

    private async Task<bool> BeUniqueName(int id, string name, CancellationToken cancellation)
    {
        var exitingBrand = await _unitOfWork.GetReadRepository<Brand>()
            .GetAsync(b => b.Name == name && b.Id != id, cancellationToken: cancellation);

        return exitingBrand == null;
    }
}
