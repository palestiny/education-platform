using EducationPlatform.Application.FirstSlice;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IFirstSliceStore, InMemoryFirstSliceStore>();
builder.Services.AddSingleton<FirstSliceService>();

var app = builder.Build();

app.MapPost("/api/v1/learning-contexts/{contextId}/assignments",
    (HttpContext http, FirstSliceService service, string contextId, AssignmentRequest request) =>
    {
        var auth = Authenticate(http);
        if (auth is null) return Results.Json(Error("AUTHENTICATION_REQUIRED"), statusCode: StatusCodes.Status401Unauthorized);
        if (auth is not ("authorized-teacher" or "teacher-without-authority" or "tenant-a-teacher"))
            return Results.Json(Error("FORBIDDEN"), statusCode: StatusCodes.Status403Forbidden);
        if (auth == "tenant-a-teacher") return Results.NotFound();

        try
        {
            var assignment = service.CreateAssignment(
                "tenant-a", auth, contextId, request.GoalId, request.LearnerId,
                request.Work.GetRawText(), http.Request.Headers["Idempotency-Key"].FirstOrDefault());
            return Results.Created($"/api/v1/assignments/{assignment.Id}", assignment);
        }
        catch (InvalidOperationException ex) when (ex.Message == "IDEMPOTENCY_CONFLICT")
        {
            return Results.Json(Error("IDEMPOTENCY_CONFLICT"), statusCode: StatusCodes.Status409Conflict);
        }
    });

app.MapPost("/api/v1/assignments/{assignmentId}/submissions",
    (HttpContext http, FirstSliceService service, string assignmentId, SubmissionRequest request) =>
    {
        var auth = Authenticate(http);
        if (auth is null) return Results.Json(Error("AUTHENTICATION_REQUIRED"), statusCode: StatusCodes.Status401Unauthorized);

        var assignment = serviceStore(http.RequestServices).GetAssignment(assignmentId);
        if (assignment is null) return Results.NotFound();
        if (auth != assignment.LearnerId) return Results.NotFound();

        try
        {
            var submission = service.CreateSubmission(
                assignment.TenantId, auth, assignment, auth, request.Payload.GetRawText(),
                http.Request.Headers["Idempotency-Key"].FirstOrDefault());
            return Results.Created($"/api/v1/submissions/{submission.Id}", submission);
        }
        catch (InvalidOperationException ex) when (ex.Message == "BUSINESS_RULE_VIOLATION")
        {
            return Results.Json(Error("BUSINESS_RULE_VIOLATION"), statusCode: StatusCodes.Status422UnprocessableEntity);
        }
        catch (InvalidOperationException ex) when (ex.Message == "IDEMPOTENCY_CONFLICT")
        {
            return Results.Json(Error("IDEMPOTENCY_CONFLICT"), statusCode: StatusCodes.Status409Conflict);
        }
    });

app.Run();

static string? Authenticate(HttpContext http)
{
    if (!http.Request.Headers.TryGetValue("Authorization", out var value)) return null;
    const string prefix = "Bearer ";
    var raw = value.ToString();
    return raw.StartsWith(prefix, StringComparison.Ordinal) ? raw[prefix.Length..] : null;
}

static object Error(string code) => new { code };

static IFirstSliceStore serviceStore(IServiceProvider services) =>
    services.GetRequiredService<IFirstSliceStore>();

public sealed record AssignmentRequest(string GoalId, string LearnerId, JsonElement Work);
public sealed record SubmissionRequest(JsonElement Payload);
public partial class Program { }
