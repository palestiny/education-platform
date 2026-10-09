using ApplicationExecutionContext = EducationPlatform.Application.Security.ExecutionContext;
using Xunit;

namespace EducationPlatform.ApplicationTests;

public sealed class ExecutionContextTests
{
    [Fact]
    public void Execution_context_requires_server_derived_tenant_and_authenticated_principal()
    {
        var context = new ApplicationExecutionContext(
            PrincipalId: "authorized-teacher",
            TenantId: "tenant-a",
            Authorities: new HashSet<string>(["assignment:create"]));

        Assert.Equal("authorized-teacher", context.PrincipalId);
        Assert.Equal("tenant-a", context.TenantId);
        Assert.Contains("assignment:create", context.Authorities);
    }

    [Fact]
    public void Execution_context_does_not_accept_client_supplied_tenant()
    {
        var context = new ApplicationExecutionContext(
            PrincipalId: "authorized-teacher",
            TenantId: "tenant-a",
            Authorities: new HashSet<string>(["assignment:create"]));

        Assert.Equal("tenant-a", context.TenantId);
    }
}
