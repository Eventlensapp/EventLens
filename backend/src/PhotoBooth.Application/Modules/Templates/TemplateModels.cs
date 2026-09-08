namespace PhotoBooth.Application.Modules.Templates;

public sealed record TemplateResponse(Guid Id, string Name, string Definition);
public sealed record CreateTemplateRequest(string Name, string Definition);
