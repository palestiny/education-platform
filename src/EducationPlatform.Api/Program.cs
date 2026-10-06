using EducationPlatform.Api.Security;
using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Application.Security;
using EducationPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EducationPlatform");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<EducationPlatformDbContext>(options =>
        options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsAssembly(typeof(EducationPlatformDbContext).Assembly.FullName)));
    builder.Services.AddScoped<IFirstSliceStore, PostgresFirstSliceStore>();
}
else if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSingleton<IFirstSliceStore, InMemoryFirstSliceStore>();
}
else
{
    throw new InvalidOperationException(
        "A PostgreSQL connection string is required outside Development/Testing.");
}

builder.Services.AddScoped<FirstSliceService>();
builder.Services.AddScoped<AssignmentLookup>();
builder.Services.AddScoped<RequestExecutionContextAccessor>();
builder.Services.AddScoped<IExecutionContextAccessor>(sp => sp.GetRequiredService<RequestExecutionContextAccessor>());

var app = builder.Build();

app.Use(async (http, next) =>
{
    var correlationId = http.Request.Headers["X-Correlation-ID"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(correlationId))
        correlationId = Guid.NewGuid().ToString("N");

    http.Items["CorrelationId"] = correlationId;
    http.Response.Headers["X-Correlation-ID"] = correlationId;

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
    {
        var executionContext = TestBearerExecutionContextResolver.Resolve(http);
        if (executionContext is not null)
            http.RequestServices.GetRequiredService<RequestExecutionContextAccessor>().Set(executionContext);
    }

    await next();
});

app.MapPost("/api/v1/learning-contexts/{contextId}/assignments",
    (HttpContext http, IExecutionContextAccessor context, FirstSliceService service, string contextId, [FromBody] AssignmentRequest request) =>
    {
        var auth = context.Current;
        if (auth is null)
            return Results.Json(Error("AUTHENTICATION_REQUIRED"), statusCode: StatusCodes.Status401Unauthorized);
        if (!auth.HasAuthority("assignment:create"))
            return Results.Json(Error("FORBIDDEN"), statusCode: StatusCodes.Status403Forbidden);
        if (contextId == "context-a" && auth.TenantId != "tenant-a")
            return Results.NotFound();

        try
        {
            var mutation = service.CreateAssignment(
                auth.TenantId, auth.PrincipalId, contextId, request.GoalId, request.LearnerId,
                request.Work.GetRawText(), http.Request.Headers["Idempotency-Key"].FirstOrDefault(),
                (string)http.Items["CorrelationId"]!);
            var assignment = mutation.Value;
            return Results.Created($"/api/v1/assignments/{assignment.Id}", assignment);
        }
        catch (InvalidOperationException ex) when (ex.Message == "IDEMPOTENCY_CONFLICT")
        {
            return Results.Json(Error("IDEMPOTENCY_CONFLICT"), statusCode: StatusCodes.Status409Conflict);
        }
    });

app.MapPost("/api/v1/assignments/{assignmentId}/submissions",
    (HttpContext http, IExecutionContextAccessor context, FirstSliceService service, string assignmentId, [FromServices] AssignmentLookup lookup, [FromBody] SubmissionRequest request) =>
    {
        var auth = context.Current;
        if (auth is null)
            return Results.Json(Error("AUTHENTICATION_REQUIRED"), statusCode: StatusCodes.Status401Unauthorized);
        if (!auth.HasAuthority("submission:create"))
            return Results.Json(Error("FORBIDDEN"), statusCode: StatusCodes.Status403Forbidden);

        var assignment = lookup.Get(assignmentId);
        if (assignment is null || assignment.TenantId != auth.TenantId) return Results.NotFound();
        if (auth.PrincipalId != assignment.LearnerId) return Results.NotFound();

        try
        {
            var mutation = service.CreateSubmission(
                auth.TenantId, auth.PrincipalId, assignment, auth.PrincipalId, request.Payload.GetRawText(),
                http.Request.Headers["Idempotency-Key"].FirstOrDefault(),
                (string)http.Items["CorrelationId"]!);
            var submission = mutation.Value;
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

app.MapPost("/api/v1/assignments/{assignmentId}/close",
    (HttpContext http, IExecutionContextAccessor context, FirstSliceService service, AssignmentLookup lookup, string assignmentId, [FromBody] CloseAssignmentRequest request) =>
    {
        var auth = context.Current;
        if (auth is null)
            return Results.Json(Error("AUTHENTICATION_REQUIRED"), statusCode: StatusCodes.Status401Unauthorized);
        if (!auth.HasAuthority("assignment:close"))
            return Results.Json(Error("FORBIDDEN"), statusCode: StatusCodes.Status403Forbidden);

        var assignment = lookup.Get(assignmentId);
        if (assignment is null || assignment.TenantId != auth.TenantId)
            return Results.NotFound();

        try
        {
            var mutation = service.CloseAssignment(
                auth.TenantId, auth.PrincipalId, assignmentId, request.ExpectedVersion,
                http.Request.Headers["Idempotency-Key"].FirstOrDefault(),
                (string)http.Items["CorrelationId"]!);
            return Results.Ok(mutation);
        }
        catch (InvalidOperationException ex) when (ex.Message == "RESOURCE_NOT_FOUND")
        {
            return Results.NotFound();
        }
        catch (InvalidOperationException ex) when (ex.Message == "CONCURRENCY_CONFLICT")
        {
            return Results.Json(Error("CONCURRENCY_CONFLICT"), statusCode: StatusCodes.Status409Conflict);
        }
        catch (InvalidOperationException ex) when (ex.Message == "IDEMPOTENCY_CONFLICT")
        {
            return Results.Json(Error("IDEMPOTENCY_CONFLICT"), statusCode: StatusCodes.Status409Conflict);
        }
        catch (InvalidOperationException ex) when (ex.Message == "VALIDATION_FAILED")
        {
            return Results.Json(Error("VALIDATION_FAILED"), statusCode: StatusCodes.Status422UnprocessableEntity);
        }
    });

app.Run();

static object Error(string code) => new { code };

public sealed class AssignmentLookup(IFirstSliceStore store)
{
    public EducationPlatform.Domain.FirstSlice.Assignment? Get(string id) => store.GetAssignment(id);
}

public sealed record AssignmentRequest(string GoalId, string LearnerId, JsonElement Work);
public sealed record SubmissionRequest(JsonElement Payload);
public sealed record CloseAssignmentRequest(int ExpectedVersion);
public partial class Program { }
