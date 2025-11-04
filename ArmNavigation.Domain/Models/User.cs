using ArmNavigation.Domain.Enums;

namespace ArmNavigation.Domain.Models
{
    public sealed class User
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public Guid MedInstitutionId { get; set; }
        public Role Role { get; set; }
        public bool IsRemoved { get; set; }
    }
}
