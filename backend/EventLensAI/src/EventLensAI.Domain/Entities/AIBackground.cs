using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;
public sealed class AIBackground : BaseEntity
{
    private AIBackground(){}
    public AIBackground(Guid? organizationId,string name,BackgroundCategory category,string imageUrl)
    {OrganizationId=organizationId;Name=name;Category=category;ImageUrl=imageUrl;}
    public Guid? OrganizationId{get;private set;}
    public Organization? Organization{get;private set;}
    public string Name{get;private set;}=string.Empty;
    public BackgroundCategory Category{get;private set;}
    public string ImageUrl{get;private set;}=string.Empty;
}
