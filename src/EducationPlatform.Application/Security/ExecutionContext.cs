namespace EducationPlatform.Application.Security;

public sealed record ExecutionContext(
    string PrincipalId,
    string TenantId,
    IReadOnlySet<string> Authorities)
{
    public bool HasAuthority(string authority) => Authorities.Contains(authority);
}
