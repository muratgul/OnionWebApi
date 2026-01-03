namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommandRequest>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka Adı boş geçilemez.")
            .MaximumLength(50).WithMessage("Marka Adı en fazla 50 karakter olabilir.")
            .MinimumLength(2).WithMessage("Marka Adı en az 2 karakter olabilir.")
            .WithName("Marka Adı");
    }
}
