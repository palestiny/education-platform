using ApplicationExecutionContext = EducationPlatform.Application.Security.ExecutionContext;

namespace EducationPlatform.Api.Security;

public static class TestBearerExecutionContextResolver
{
    public static ApplicationExecutionContext? Resolve(HttpContext http)
    {
        if (!http.Request.Headers.TryGetValue("Authorization", out var value))
            return null;

        const string prefix = "Bearer ";
        var raw = value.ToString();
        if (!raw.StartsWith(prefix, StringComparison.Ordinal))
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
            _ => null
        };
    }
}
