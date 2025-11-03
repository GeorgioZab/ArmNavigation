namespace ArmNavigation.Domain.Models
{
    public class MedInstitution
    {
        public Guid MedInstitutionId { get; set; }
        public string Name { get; set; } = default!;
        public bool IsRemoved { get; set; } = false;
    }
}
