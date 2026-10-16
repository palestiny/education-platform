using System.Net;
using System.Net.Http.Json;
using EducationPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EducationPlatform.IntegrationTests;

[Collection("PostgresPersistence")]
public sealed class DurableApiFirstSliceTests
{
    [Fact]
    public async Task Assignment_api_persists_authoritative_and_reliability_records()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        request.Headers.Add("Idempotency-Key", "durable-api-assignment");
        request.Headers.Add("X-Correlation-ID", "durable-correlation");

        request.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "durable" }
        });

        var first = await client.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal("durable-correlation", first.Headers.GetValues("X-Correlation-ID").Single());

        var firstBody = await first.Content.ReadFromJsonAsync<AssignmentResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(firstBody);

        using var retry = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");
        retry.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        retry.Headers.Add("Idempotency-Key", "durable-api-assignment");
        retry.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "durable" }
        });

        var second = await client.SendAsync(retry, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var secondBody = await second.Content.ReadFromJsonAsync<AssignmentResponse>(TestContext.Current.CancellationToken);
        Assert.Equal(firstBody.Id, secondBody!.Id);

        db.ChangeTracker.Clear();
        Assert.Equal(1, db.Assignments.Count());
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());

        var assignment = db.Assignments.Single();
        var audit = db.AuditRecords.Single();
        var outbox = db.OutboxMessages.Single();

        Assert.Equal(firstBody!.Id, assignment.Id);
        Assert.Equal("tenant-a", audit.TenantId);
        Assert.Equal("authorized-teacher", audit.ActorId);
        Assert.Equal("context-a", audit.ContextId);
        Assert.Equal("assignment.create", audit.Operation);
        Assert.Equal("durable-correlation", audit.CorrelationId);
        Assert.Equal("AssignmentCreated", audit.ResultingStatus);
        Assert.Equal("PENDING", outbox.Status);
    }

    [Fact]
    public async Task Production_environment_rejects_test_bearer_without_mutating_data()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection);
            });

        using var client = factory.CreateClient();

        var requests = new[]
        {
            new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/learning-contexts/context-a/assignments")
            {
                Content = JsonContent.Create(new
                {
                    goalId = "goal-a",
                    learnerId = "learner-a",
                    work = new { prompt = "must not persist" }
                })
            },
            new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/assignments/assignment-a/submissions")
            {
                Content = JsonContent.Create(new
                {
                    payload = new { answer = "must not persist" }
                })
            },
            new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/assignments/assignment-a/close")
            {
                Content = JsonContent.Create(new { expectedVersion = 1 })
            }
        };

        try
        {
            for (var index = 0; index < requests.Length; index++)
            {
                var request = requests[index];
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
                request.Headers.Add("Idempotency-Key", $"production-must-fail-closed-{index}");

                var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }
        finally
        {
            foreach (var request in requests)
                request.Dispose();
        }

        db.ChangeTracker.Clear();
        Assert.Empty(db.Assignments);
        Assert.Empty(db.Submissions);
        Assert.Empty(db.IdempotencyRecords);
        Assert.Empty(db.AuditRecords);
        Assert.Empty(db.OutboxMessages);
    }

    [Fact]
    public async Task Api_rejects_missing_authority_and_cross_tenant_context_without_mutating_data()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();

        async Task<HttpStatusCode> SendAssignmentRequest(string bearer, string idempotencyKey)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/learning-contexts/context-a/assignments");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
            request.Headers.Add("Idempotency-Key", idempotencyKey);
            request.Content = JsonContent.Create(new
            {
                goalId = "goal-a",
                learnerId = "learner-a",
                work = new { prompt = "must not persist" }
            });

            using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
            return response.StatusCode;
        }

        Assert.Equal(
            HttpStatusCode.Forbidden,
            await SendAssignmentRequest("teacher-without-authority", "missing-authority"));
        Assert.Equal(
            HttpStatusCode.NotFound,
            await SendAssignmentRequest("tenant-a-teacher", "cross-tenant"));

        db.ChangeTracker.Clear();
        Assert.Empty(db.Assignments);
        Assert.Empty(db.Submissions);
        Assert.Empty(db.IdempotencyRecords);
        Assert.Empty(db.AuditRecords);
        Assert.Empty(db.OutboxMessages);
    }

    [Theory]
    [InlineData("close-member-without-grant", HttpStatusCode.Forbidden)]
    [InlineData("close-grant-without-membership", HttpStatusCode.Forbidden)]
    [InlineData("close-authorization-indeterminate", HttpStatusCode.ServiceUnavailable)]
    public async Task Assignment_close_denial_or_indeterminate_authorization_does_not_mutate_data(
        string bearer,
        HttpStatusCode expectedStatus)
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();
        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");
        createRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        createRequest.Headers.Add("Idempotency-Key", "close-authorization-create");
        createRequest.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "authorization boundary" }
        });

        using var createResponse = await client.SendAsync(
            createRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<AssignmentResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        db.ChangeTracker.Clear();
        Assert.Equal(1, db.Assignments.Count());
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());

        using var closeRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{created.Id}/close");
        closeRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
        closeRequest.Headers.Add("Idempotency-Key", $"close-authorization-{bearer}");
        closeRequest.Content = JsonContent.Create(new { expectedVersion = 1 });

        using var closeResponse = await client.SendAsync(
            closeRequest, TestContext.Current.CancellationToken);
        Assert.Equal(expectedStatus, closeResponse.StatusCode);

        db.ChangeTracker.Clear();
        var assignment = db.Assignments.Single();
        Assert.Equal(created.Id, assignment.Id);
        Assert.Equal("ACTIVE", assignment.Status);
        Assert.Equal(1, assignment.Version);
        Assert.Empty(db.Submissions);
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());
    }

    [Fact]
    public async Task Assignment_close_replay_is_denied_after_actor_loses_resource_grant()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();
        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");
        createRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        createRequest.Headers.Add("Idempotency-Key", "close-replay-create");
        createRequest.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "protected replay" }
        });

        using var createResponse = await client.SendAsync(
            createRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<AssignmentResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        using var closeRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{created.Id}/close");
        closeRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        closeRequest.Headers.Add("Idempotency-Key", "protected-close-replay");
        closeRequest.Content = JsonContent.Create(new { expectedVersion = 1 });

        using var firstClose = await client.SendAsync(
            closeRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, firstClose.StatusCode);

        db.ChangeTracker.Clear();
        Assert.Equal("CLOSED", db.Assignments.Single().Status);
        Assert.Equal(2, db.Assignments.Single().Version);
        Assert.Equal(2, db.IdempotencyRecords.Count());
        Assert.Equal(2, db.AuditRecords.Count());
        Assert.Equal(2, db.OutboxMessages.Count());

        using var replayRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{created.Id}/close");
        replayRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "close-member-without-grant");
        replayRequest.Headers.Add("Idempotency-Key", "protected-close-replay");
        replayRequest.Content = JsonContent.Create(new { expectedVersion = 1 });

        using var replayResponse = await client.SendAsync(
            replayRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Forbidden, replayResponse.StatusCode);
        var deniedBody = await replayResponse.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken);
        Assert.DoesNotContain(created.Id, deniedBody, StringComparison.Ordinal);

        db.ChangeTracker.Clear();
        var assignment = db.Assignments.Single();
        Assert.Equal("CLOSED", assignment.Status);
        Assert.Equal(2, assignment.Version);
        Assert.Empty(db.Submissions);
        Assert.Equal(2, db.IdempotencyRecords.Count());
        Assert.Equal(2, db.AuditRecords.Count());
        Assert.Equal(2, db.OutboxMessages.Count());
    }

    [Fact]
    public async Task Learner_cannot_submit_for_another_learner_without_mutating_data()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();
        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");
        createRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        createRequest.Headers.Add("Idempotency-Key", "learner-boundary-create");
        createRequest.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "only the assigned learner may submit" }
        });

        using var createResponse = await client.SendAsync(
            createRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<AssignmentResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        db.ChangeTracker.Clear();
        Assert.Equal(1, db.Assignments.Count());
        Assert.Empty(db.Submissions);
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());

        using var submitRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{created.Id}/submissions");
        submitRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-learner");
        submitRequest.Headers.Add("Idempotency-Key", "wrong-learner-submit");
        submitRequest.Content = JsonContent.Create(new
        {
            payload = new { answer = "must not persist" }
        });

        using var submitResponse = await client.SendAsync(
            submitRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, submitResponse.StatusCode);

        db.ChangeTracker.Clear();
        Assert.Equal(1, db.Assignments.Count());
        Assert.Empty(db.Submissions);
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());
    }

    [Fact]
    public async Task Cross_tenant_actor_cannot_close_assignment_or_create_reliability_records()
    {
        using var db = ResetDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        await using var factory = new WebApplicationFactory<global::Program>()
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ConnectionStrings:EducationPlatform", connection));

        using var client = factory.CreateClient();
        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/learning-contexts/context-a/assignments");
        createRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "authorized-teacher");
        createRequest.Headers.Add("Idempotency-Key", "cross-tenant-close-create");
        createRequest.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId = "learner-a",
            work = new { prompt = "tenant boundary" }
        });

        using var createResponse = await client.SendAsync(
            createRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<AssignmentResponse>(
            TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        db.ChangeTracker.Clear();
        Assert.Equal("ACTIVE", db.Assignments.Single().Status);
        Assert.Equal(1, db.Assignments.Single().Version);
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());

        using var closeRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{created.Id}/close");
        closeRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "tenant-a-teacher");
        closeRequest.Headers.Add("Idempotency-Key", "cross-tenant-close-attempt");
        closeRequest.Content = JsonContent.Create(new { expectedVersion = 1 });

        using var closeResponse = await client.SendAsync(
            closeRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, closeResponse.StatusCode);

        db.ChangeTracker.Clear();
        Assert.Equal("ACTIVE", db.Assignments.Single().Status);
        Assert.Equal(1, db.Assignments.Single().Version);
        Assert.Empty(db.Submissions);
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());
    }

    private static EducationPlatformDbContext ResetDatabase()
    {
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connection, npgsql =>
                npgsql.MigrationsAssembly(typeof(EducationPlatformDbContext).Assembly.FullName))
            .Options;

        var db = new EducationPlatformDbContext(options);
        db.Database.Migrate();

        db.Assignments.RemoveRange(db.Assignments);
        db.Submissions.RemoveRange(db.Submissions);
        db.IdempotencyRecords.RemoveRange(db.IdempotencyRecords);
        db.AuditRecords.RemoveRange(db.AuditRecords);
        db.OutboxMessages.RemoveRange(db.OutboxMessages);
        db.SaveChanges();

        return db;
    }

    private sealed record AssignmentResponse(string Id);
}
