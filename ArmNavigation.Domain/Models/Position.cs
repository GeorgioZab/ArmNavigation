namespace ArmNavigation.Domain.Models
{
    public sealed record Position
    {
        public Guid PositionId { get; set; }
        public Guid CarId { get; set; }
        public DateTime Timestamp { get; set; }
        public Point Coordinates { get; set; }
    }
}
