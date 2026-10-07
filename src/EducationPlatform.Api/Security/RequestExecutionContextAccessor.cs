using ApplicationExecutionContext = EducationPlatform.Application.Security.ExecutionContext;
using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

public sealed class RequestExecutionContextAccessor : IExecutionContextAccessor
{
    private ApplicationExecutionContext? _current;

    public ApplicationExecutionContext? Current => _current;

    public void Set(ApplicationExecutionContext context) => _current = context;
}
