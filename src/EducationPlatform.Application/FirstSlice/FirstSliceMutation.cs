namespace EducationPlatform.Application.FirstSlice;

public sealed record FirstSliceMutation<T>(T Value, bool Replayed);
