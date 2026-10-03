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
