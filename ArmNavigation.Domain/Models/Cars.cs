namespace ArmNavigation.Domain.Models
{
    public sealed record Car 
    {
        public Guid CarId { get; init; }
        public string RegNum { get; init; }
        public Guid MedInstitutionId { get; init; }
        public string? GpsTracker { get; init; }
        public bool IsRemoved { get; init; }
    }
}
