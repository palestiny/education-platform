namespace EducationPlatform.Application.Security;

public interface IExecutionContextAccessor
{
    ExecutionContext? Current { get; }
}
