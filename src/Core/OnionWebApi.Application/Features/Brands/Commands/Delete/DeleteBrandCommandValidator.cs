namespace OnionWebApi.Application.Features.Brands.Commands.Delete;
public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommandRequest>
{
    public DeleteBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Brand ID is required.");
    }
}
