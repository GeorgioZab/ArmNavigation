public sealed record UserRoleInfo
{
    public int Role { get; init; }
    public Guid MedInstitutionId { get; init; }
}