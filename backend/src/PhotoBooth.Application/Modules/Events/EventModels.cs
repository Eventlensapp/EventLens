namespace PhotoBooth.Application.Modules.Events;

public sealed record EventResponse(Guid Id, string Name, DateTime StartsAtUtc, DateTime EndsAtUtc);
public sealed record CreateEventRequest(string Name, DateTime StartsAtUtc, DateTime EndsAtUtc);
