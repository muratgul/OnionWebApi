namespace OnionWebApi.Application.Beheviors;
public class FluentValidationBehevior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validator;

    public FluentValidationBehevior(IEnumerable<IValidator<TRequest>> validator)
    {
        this.validator = validator;
    }
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failtures = validator
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .GroupBy(x => x.ErrorMessage)
            .Select(x => x.First())
            .Where(f => f != null)
            .ToList();

        return failtures.Count != 0 ? throw new FluentValidation.ValidationException(failtures) : next(cancellationToken);
    }
}
