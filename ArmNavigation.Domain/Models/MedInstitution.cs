namespace ArmNavigation.Domain.Models
{
    public sealed record MedInstitution
    {
        public Guid MedInstitutionId { get; init; }
        public string Name { get; init; }
        public bool IsRemoved { get; init; }
    }
}
