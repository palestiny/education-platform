using ApplicationExecutionContext = EducationPlatform.Application.Security.ExecutionContext;

namespace EducationPlatform.Api.Security;

public sealed class TestBearerExecutionContextResolver : IExecutionContextResolver
{
    public ValueTask<ApplicationExecutionContext?> ResolveAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(Resolve(httpContext));
    }

    private static ApplicationExecutionContext? Resolve(HttpContext http)
    {
        if (!http.Request.Headers.TryGetValue("Authorization", out var value))
            return null;

        const string prefix = "Bearer ";
        var raw = value.ToString();
        if (!raw.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        return raw[prefix.Length..] switch
        {
            "authorized-teacher" => new ApplicationExecutionContext(
                "authorized-teacher", "tenant-a",
                new HashSet<string>(["assignment:create", "assignment:close"])),
            "teacher-without-authority" => new ApplicationExecutionContext(
                "teacher-without-authority", "tenant-a",
                new HashSet<string>()),
            "tenant-a-teacher" => new ApplicationExecutionContext(
                "tenant-a-teacher", "tenant-b",
                new HashSet<string>(["assignment:create", "assignment:close"])),
            "authorized-learner" => new ApplicationExecutionContext(
                "authorized-learner", "tenant-a",
                new HashSet<string>(["submission:create"])),
            "close-member-without-grant" => new ApplicationExecutionContext(
                "close-member-without-grant", "tenant-a",
                new HashSet<string>(["assignment:close"])),
            "close-grant-without-membership" => new ApplicationExecutionContext(
                "close-grant-without-membership", "tenant-a",
                new HashSet<string>(["assignment:close"])),
            "close-authorization-indeterminate" => new ApplicationExecutionContext(
                "close-authorization-indeterminate", "tenant-a",
                new HashSet<string>(["assignment:close"])),
            _ => null
        };
    }
}
