using EducationPlatform.Application.Security;
using Microsoft.AspNetCore.Http;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Production-safe default until an approved real credential-validation adapter is configured.
/// Never treats a bearer string as an authenticated identity.
/// </summary>
public sealed class FailClosedExecutionContextResolver : IExecutionContextResolver
{
    public ValueTask<EducationPlatform.Application.Security.ExecutionContext?> ResolveAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult<EducationPlatform.Application.Security.ExecutionContext?>(null);
    }
}
