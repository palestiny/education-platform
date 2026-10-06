using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

public sealed class RequestExecutionContextAccessor : IExecutionContextAccessor
{
    private ExecutionContext? _current;

    public ExecutionContext? Current => _current;

    public void Set(ExecutionContext context) => _current = context;
}
