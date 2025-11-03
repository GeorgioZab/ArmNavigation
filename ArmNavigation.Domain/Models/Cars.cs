using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmNavigation.Domain.Models
{
    public class Car
    {
        public Guid CarId { get; set; }
        public string RegNum { get; set; } = default!;
        public Guid MedInstitutionId { get; set; }
        public string? GpsTracker { get; set; }
        public bool IsRemoved { get; set; } = false;
    }
}
