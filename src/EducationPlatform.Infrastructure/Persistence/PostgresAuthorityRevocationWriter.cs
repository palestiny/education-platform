using Microsoft.EntityFrameworkCore;

namespace EducationPlatform.Infrastructure.Persistence;

/// <summary>
/// Platform-owned authority mutation path. Revocation takes the same tenant/context
/// lock used by protected commands, so commands and revocations serialize consistently.
/// All future membership/grant/context lifecycle writers must follow this protocol.
/// </summary>
public sealed class PostgresAuthorityRevocationWriter(EducationPlatformDbContext db)
{
    public async Task RevokeGrantAsync(
        string tenantId, string contextId, string subjectType, string subjectId, string action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await LockContextAsync(tenantId, contextId, cancellationToken);
            var grant = await db.ActionPolicyGrants.SingleOrDefaultAsync(x =>
                x.TenantId == tenantId && x.ContextId == contextId &&
                x.SubjectType == subjectType && x.SubjectId == subjectId && x.Action == action,
                cancellationToken);
            if (grant is null)
                throw new InvalidOperationException("AUTHORITY_RECORD_NOT_FOUND");

            grant.Status = "REVOKED";
            grant.Version++;
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task RevokeMembershipAsync(
        string tenantId, string contextId, string personId, string membershipType,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await LockContextAsync(tenantId, contextId, cancellationToken);
            var membership = await db.ContextualMemberships.SingleOrDefaultAsync(x =>
                x.TenantId == tenantId && x.ContextId == contextId &&
                x.PersonId == personId && x.MembershipType == membershipType,
                cancellationToken);
            if (membership is null)
                throw new InvalidOperationException("AUTHORITY_RECORD_NOT_FOUND");

            membership.Status = "REVOKED";
            membership.Version++;
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    private async Task LockContextAsync(
        string tenantId, string contextId, CancellationToken cancellationToken)
    {
        var context = await db.LearningContexts.FromSqlInterpolated(
                $"SELECT * FROM learning_contexts WHERE \"TenantId\" = {tenantId} AND \"Id\" = {contextId} FOR UPDATE")
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);
        if (context is null)
            throw new InvalidOperationException("AUTHORITY_CONTEXT_NOT_FOUND");
    }
}
