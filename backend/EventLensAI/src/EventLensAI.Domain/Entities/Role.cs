using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class Role : BaseEntity
{
    private Role() { }
    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    public string Name { get; private set; } = string.Empty;
    public ICollection<OrganizationMember> Members { get; private set; } = [];
}

public static class SystemRoles
{
    public static readonly Guid SuperAdminId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid OwnerId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid ManagerId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid PhotographerId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    public static readonly Guid GuestId = Guid.Parse("10000000-0000-0000-0000-000000000005");
    public static readonly Guid EditorId = Guid.Parse("10000000-0000-0000-0000-000000000006");
    public static readonly Guid ViewerId = Guid.Parse("10000000-0000-0000-0000-000000000007");
    public static readonly Guid BoothOperatorId = Guid.Parse("10000000-0000-0000-0000-000000000008");
    public static readonly Guid DesignerId = Guid.Parse("10000000-0000-0000-0000-000000000009");
    public static readonly Guid MarketingManagerId = Guid.Parse("10000000-0000-0000-0000-000000000010");
    public static readonly Guid PlatformAdminId = Guid.Parse("10000000-0000-0000-0000-000000000011");
    public const string SuperAdmin = "SuperAdmin";
    public const string Owner = "Owner";
    public const string Manager = "Manager";
    public const string Photographer = "Photographer";
    public const string Guest = "Guest";
    public const string Editor = "Editor";
    public const string Viewer = "Viewer";
    public const string BoothOperator = "BoothOperator";
    public const string Designer = "Designer";
    public const string MarketingManager = "MarketingManager";
    public const string PlatformAdmin = "PlatformAdmin";
    public const string OrganizationOwner = Owner;
}
