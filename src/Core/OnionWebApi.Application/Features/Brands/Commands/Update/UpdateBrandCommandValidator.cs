namespace OnionWebApi.Application.Features.Brands.Commands.Update;
internal class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommandRequest>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Brand ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name must not exceed 100 characters.");
    }
}
