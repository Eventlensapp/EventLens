using PhotoBooth.Domain.Common;

namespace PhotoBooth.Domain.Entities;

public sealed class Template : Entity
{
    private Template() { }
    public Template(string name, string definition)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required.") : name.Trim();
        Definition = string.IsNullOrWhiteSpace(definition) ? throw new ArgumentException("Definition is required.") : definition;
    }

    public string Name { get; private set; } = string.Empty;
    public string Definition { get; private set; } = string.Empty;
    public ICollection<Photo> Photos { get; private set; } = [];
}
