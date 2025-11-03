using ArmNavigation.Domain.Enums;

namespace ArmNavigation.Domain.Models
{
    public class User
    {
        public Guid UsernId { get; set; }
        public string Login { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public Guid MedInstitutionId { get; set; }
        public Role Role { get; set; }
        public bool IsRemoved { get; set; } = false;
    }
}
