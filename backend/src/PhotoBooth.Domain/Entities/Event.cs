using PhotoBooth.Domain.Common;

namespace PhotoBooth.Domain.Entities;

public sealed class Event : Entity
{
    private Event() { }

    public Event(string name, DateTime startsAtUtc, DateTime endsAtUtc)
    {
        Update(name, startsAtUtc, endsAtUtc);
    }

    public string Name { get; private set; } = string.Empty;
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public ICollection<Photo> Photos { get; private set; } = [];

    public void Update(string name, DateTime startsAtUtc, DateTime endsAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (endsAtUtc <= startsAtUtc) throw new ArgumentException("End time must follow start time.");
        Name = name.Trim();
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        MarkUpdated();
    }
}
