using System.Collections.Frozen;
using EducationPlatform.Application.Security;
using ExecutionContext = EducationPlatform.Application.Security.ExecutionContext;

namespace EducationPlatform.Application.Identity;

/// <summary>A validated issuer/subject pair produced only by the API authentication boundary.</summary>
public sealed record ValidatedExternalIdentity(string Issuer, string Subject);

public enum IdentityResolutionOutcome
{
    AuthorizedContext,
    UnknownIdentity,
    PersonInactive,
    NoEligibleMembership,
    AmbiguousMembership,
    Denied,
    Indeterminate
}

public sealed record PersonIdentityResolution(IdentityResolutionOutcome Outcome, string? PersonId = null);

public enum PersonLifecycleState
{
    Active,
    Inactive,
    Indeterminate
}

public sealed record ContextualMembershipResolution(
    IdentityResolutionOutcome Outcome,
    string? TenantId = null,
    string? LearningContextId = null,
    IReadOnlySet<string>? Authorities = null);

public sealed record ResourceActionAuthorizationRequest(
    string PersonId,
    string TenantId,
    string LearningContextId,
    string ResourceId,
    string Action);

public interface IExternalIdentityPersonResolver
{
    ValueTask<PersonIdentityResolution> ResolvePersonAsync(
        ValidatedExternalIdentity identity,
        CancellationToken cancellationToken = default);
}

public interface IPersonLifecycleResolver
{
    ValueTask<PersonLifecycleState> GetLifecycleStateAsync(
        string personId,
        CancellationToken cancellationToken = default);
}

public interface IContextualMembershipResolver
{
    ValueTask<ContextualMembershipResolution> ResolveMembershipAsync(
        string personId,
        string resourceId,
        string action,
        CancellationToken cancellationToken = default);
}

public interface IResourceActionPolicy
{
    ValueTask<AuthorizationDecision> AuthorizeAsync(
        ResourceActionAuthorizationRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record IdentityExecutionContextResolution(
    IdentityResolutionOutcome Outcome,
    ExecutionContext? Context = null);

/// <summary>
/// Provider-neutral, fail-closed orchestration. It consumes validated identity data,
/// never raw credentials, and creates context only after every stage explicitly succeeds.
/// </summary>
public sealed class IdentityExecutionContextResolver(
    IExternalIdentityPersonResolver identityResolver,
    IPersonLifecycleResolver lifecycleResolver,
    IContextualMembershipResolver membershipResolver,
    IResourceActionPolicy policy)
{
    public async ValueTask<IdentityExecutionContextResolution> ResolveAsync(
        ValidatedExternalIdentity identity,
        string resourceId,
        string action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        cancellationToken.ThrowIfCancellationRequested();

        var person = await identityResolver.ResolvePersonAsync(identity, cancellationToken);
        if (person.Outcome != IdentityResolutionOutcome.AuthorizedContext)
            return Failure(person.Outcome);
        if (string.IsNullOrWhiteSpace(person.PersonId))
            return Failure(IdentityResolutionOutcome.Indeterminate);

        var lifecycle = await lifecycleResolver.GetLifecycleStateAsync(person.PersonId, cancellationToken);
        if (lifecycle == PersonLifecycleState.Inactive)
            return Failure(IdentityResolutionOutcome.PersonInactive);
        if (lifecycle != PersonLifecycleState.Active)
            return Failure(IdentityResolutionOutcome.Indeterminate);

        var membership = await membershipResolver.ResolveMembershipAsync(
            person.PersonId, resourceId, action, cancellationToken);
        if (membership.Outcome != IdentityResolutionOutcome.AuthorizedContext)
            return Failure(membership.Outcome);
        if (string.IsNullOrWhiteSpace(membership.TenantId) ||
            string.IsNullOrWhiteSpace(membership.LearningContextId) ||
            membership.Authorities is null)
            return Failure(IdentityResolutionOutcome.Indeterminate);

        var authorization = await policy.AuthorizeAsync(
            new ResourceActionAuthorizationRequest(
                person.PersonId,
                membership.TenantId,
                membership.LearningContextId,
                resourceId,
                action),
            cancellationToken);

        if (authorization == AuthorizationDecision.Denied)
            return Failure(IdentityResolutionOutcome.Denied);
        if (authorization != AuthorizationDecision.Allowed)
            return Failure(IdentityResolutionOutcome.Indeterminate);

        return new IdentityExecutionContextResolution(
            IdentityResolutionOutcome.AuthorizedContext,
            new ExecutionContext(
                person.PersonId,
                membership.TenantId,
                membership.Authorities.ToFrozenSet(StringComparer.Ordinal)));
    }

    private static IdentityExecutionContextResolution Failure(IdentityResolutionOutcome outcome) =>
        new(outcome == IdentityResolutionOutcome.AuthorizedContext
            ? IdentityResolutionOutcome.Indeterminate
            : outcome);
}
