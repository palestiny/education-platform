using EducationPlatform.Application.Security;
using Microsoft.AspNetCore.Http;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Provider-neutral boundary between HTTP credential handling and the application's
/// trusted execution context. Provider SDK types and raw credentials must not escape this adapter.
/// </summary>
public interface IExecutionContextResolver
{
    ValueTask<EducationPlatform.Application.Security.ExecutionContext?> ResolveAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken = default);
}
