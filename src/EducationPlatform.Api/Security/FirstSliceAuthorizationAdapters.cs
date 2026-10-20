using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Deterministic fixture only. This is not a durable membership or resource-policy adapter.
/// </summary>
public sealed class TestFixtureAssignmentCreationAuthorizer : IAssignmentCreationAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCreationAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var allowed = request.PrincipalId == "authorized-teacher" &&
            request.TenantId == "tenant-a" &&
            request.LearningContextId == "context-a" &&
            request.GoalId == "goal-a" &&
            !string.IsNullOrWhiteSpace(request.LearnerId) &&
            request.Action == "assignment.create";

        return ValueTask.FromResult(allowed
            ? AuthorizationDecision.Allowed
            : AuthorizationDecision.Denied);
    }
}

/// <summary>
/// Production-safe default until durable membership and resource-policy adapters are approved.
/// </summary>
public sealed class FailClosedAssignmentCreationAuthorizer : IAssignmentCreationAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCreationAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(AuthorizationDecision.Indeterminate);
    }
}

/// <summary>
/// Deterministic fixture only. This is not a durable membership or resource-policy adapter.
/// </summary>
public sealed class TestFixtureLearnerSubmissionAuthorizer : ILearnerSubmissionAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        LearnerSubmissionAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var allowed = request.PrincipalId == "authorized-learner" &&
            request.LearnerId == request.PrincipalId &&
            request.TenantId == "tenant-a" &&
            request.LearningContextId == "context-a" &&
            request.Action == "submission.create";

        return ValueTask.FromResult(allowed
            ? AuthorizationDecision.Allowed
            : AuthorizationDecision.Denied);
    }
}

/// <summary>
/// Production-safe default until durable membership and resource-policy adapters are approved.
/// </summary>
public sealed class FailClosedLearnerSubmissionAuthorizer : ILearnerSubmissionAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        LearnerSubmissionAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(AuthorizationDecision.Indeterminate);
    }
}
