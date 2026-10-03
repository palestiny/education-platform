using Xunit;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EducationPlatform.ApplicationTests;

public sealed class FirstSliceRedTests : IClassFixture<WebApplicationFactory<global::Program>>
{
    private readonly HttpClient _client;

    public FirstSliceRedTests(WebApplicationFactory<global::Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RED_001_Unauthenticated_assignment_creation_is_rejected()
    {
        var response = await PostAssignment();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RED_002_Authenticated_unauthorized_teacher_is_rejected()
    {
        var response = await PostAssignment("teacher-without-authority");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RED_003_Authorized_teacher_creates_assignment()
    {
        var response = await PostAssignment("authorized-teacher");
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RED_004_Same_idempotency_key_replays_original_assignment()
    {
        const string key = "red-004-idempotency";
        var first = await PostAssignment("authorized-teacher", key);
        var second = await PostAssignment("authorized-teacher", key);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(first.StatusCode, second.StatusCode);
        Assert.Equal(
            await first.Content.ReadAsStringAsync(TestContext.Current.CancellationToken),
            await second.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RED_005_Reusing_idempotency_key_with_different_request_conflicts()
    {
        const string key = "red-005-idempotency-conflict";
        var first = await PostAssignment("authorized-teacher", key, "learner-a");
        var second = await PostAssignment("authorized-teacher", key, "learner-b");

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task RED_006_Cross_tenant_assignment_is_rejected()
    {
        var response = await PostAssignment("tenant-a-teacher");
        Assert.True(
            response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound,
            $"Unexpected status: {(int)response.StatusCode}");
    }

    [Fact]
    public async Task RED_007_Authorized_learner_submits_active_assignment()
    {
        var response = await PostSubmission("authorized-learner");
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RED_008_Duplicate_submission_retry_does_not_duplicate()
    {
        const string key = "red-008-submission";
        var first = await PostSubmission("authorized-learner", key);
        var second = await PostSubmission("authorized-learner", key);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(first.StatusCode, second.StatusCode);
        Assert.Equal(
            await first.Content.ReadAsStringAsync(TestContext.Current.CancellationToken),
            await second.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RED_009_Closed_assignment_rejects_submission()
    {
        var response = await PostSubmission("authorized-learner", assignmentId: "closed-assignment");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }


    [Fact]
    public async Task Expected_version_prevents_stale_assignment_close()
    {
        using var first = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/assignments/assignment-a/close");
        first.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", "authorized-teacher");
        first.Headers.Add("Idempotency-Key", "close-first");
        first.Content = JsonContent.Create(new { expectedVersion = 1 });

        var firstResponse = await _client.SendAsync(
            first, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var second = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/assignments/assignment-a/close");
        second.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", "authorized-teacher");
        second.Headers.Add("Idempotency-Key", "close-second");
        second.Content = JsonContent.Create(new { expectedVersion = 1 });

        var secondResponse = await _client.SendAsync(
            second, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.Equal(
            "CONCURRENCY_CONFLICT",
            await secondResponse.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken));
    }

    private async Task<HttpResponseMessage> PostAssignment(
        string? actor = null,
        string? idempotencyKey = null,
        string learnerId = "learner-a")
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/learning-contexts/context-a/assignments");

        if (actor is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actor);

        if (idempotencyKey is not null)
            request.Headers.Add("Idempotency-Key", idempotencyKey);

        request.Content = JsonContent.Create(new
        {
            goalId = "goal-a",
            learnerId,
            work = new { }
        });

        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> PostSubmission(
        string actor,
        string? idempotencyKey = null,
        string assignmentId = "assignment-a")
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/assignments/{assignmentId}/submissions");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actor);

        if (idempotencyKey is not null)
            request.Headers.Add("Idempotency-Key", idempotencyKey);

        request.Content = JsonContent.Create(new { payload = new { answer = "test" } });

        return await _client.SendAsync(request);
    }
}
