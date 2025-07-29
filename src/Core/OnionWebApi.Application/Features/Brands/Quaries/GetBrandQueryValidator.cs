namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetBrandQueryValidator : AbstractValidator<GetBrandQueryRequest>
{
    public GetBrandQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id cannot be empty")
            .GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}
