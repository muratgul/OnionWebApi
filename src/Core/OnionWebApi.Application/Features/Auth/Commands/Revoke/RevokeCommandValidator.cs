namespace OnionWebApi.Application.Features.Auth.Commands.Revoke;
public class RevokeCommandValidator : AbstractValidator<RevokeCommandRequest>
{
    public RevokeCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();
    }
}
