namespace OnionWebApi.Api.Extensions;

public static class AuthorizationExtensions
{
    public static TBuilder RequireAnyPermission<TBuilder>(
        this TBuilder builder, params string[] permissions)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization(policy =>
            policy.RequireAssertion(ctx =>
                permissions.Any(p => ctx.User.HasClaim("Permission", p))));
    }
}