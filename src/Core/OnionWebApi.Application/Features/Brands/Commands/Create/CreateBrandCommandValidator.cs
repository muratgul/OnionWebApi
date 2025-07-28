namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommandRequest>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MinimumLength(2)
            .WithName("Marka Adı");
    }
}
