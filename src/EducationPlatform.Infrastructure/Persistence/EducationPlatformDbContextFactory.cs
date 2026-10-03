using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EducationPlatform.Infrastructure.Persistence;

public sealed class EducationPlatformDbContextFactory : IDesignTimeDbContextFactory<EducationPlatformDbContext>
{
    public EducationPlatformDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_DESIGN_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=education_platform;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new EducationPlatformDbContext(options);
    }
}
