namespace ArmNavigation.Domain.Models
{
    public sealed record User
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public Guid MedInstitutionId { get; set; }
        public int Role { get; set; }
        public bool IsRemoved { get; set; }
    }
}
