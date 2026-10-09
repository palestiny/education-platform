using EducationPlatform.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace EducationPlatform.ApplicationTests;

public sealed class ExecutionContextResolverContractTests
{
    [Fact]
    public async Task Test_resolver_returns_no_context_for_missing_or_unrecognized_credentials()
    {
        var resolver = new TestBearerExecutionContextResolver();
        var missing = new DefaultHttpContext();
        var unknown = new DefaultHttpContext();
        unknown.Request.Headers.Authorization = "Bearer not-a-known-test-credential";

        Assert.Null(await resolver.ResolveAsync(missing, TestContext.Current.CancellationToken));
        Assert.Null(await resolver.ResolveAsync(unknown, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Test_resolver_uses_server_owned_identity_and_tenant_mapping()
    {
        var resolver = new TestBearerExecutionContextResolver();
        var http = new DefaultHttpContext();
        http.Request.Headers.Authorization = "Bearer authorized-teacher";

        var context = await resolver.ResolveAsync(http, TestContext.Current.CancellationToken);

        Assert.NotNull(context);
        Assert.Equal("authorized-teacher", context.PrincipalId);
        Assert.Equal("tenant-a", context.TenantId);
        Assert.Contains("assignment:create", context.Authorities);
        Assert.Contains("assignment:close", context.Authorities);
    }

    [Fact]
    public async Task Production_default_never_accepts_test_bearer_credentials()
    {
        var resolver = new FailClosedExecutionContextResolver();
        var http = new DefaultHttpContext();
        http.Request.Headers.Authorization = "Bearer authorized-teacher";

        var context = await resolver.ResolveAsync(http, TestContext.Current.CancellationToken);

        Assert.Null(context);
    }

    [Fact]
    public async Task Resolvers_honor_request_cancellation()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var http = new DefaultHttpContext();
        var testResolver = new TestBearerExecutionContextResolver();
        var productionResolver = new FailClosedExecutionContextResolver();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await testResolver.ResolveAsync(http, source.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await productionResolver.ResolveAsync(http, source.Token));
    }
}
