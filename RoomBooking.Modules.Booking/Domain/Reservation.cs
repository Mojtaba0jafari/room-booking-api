using RoomBooking.SharedKernel;
namespace RoomBooking.Modules.Booking.Domain
{
    public sealed class Reservation : Entity
    {
        public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

        public Guid RoomId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTimeOffset StartsAt { get; private set; }
        public DateTimeOffset EndsAt { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Reservation()
        {
        }

        private Reservation(
            Guid id,
            Guid roomId,
            Guid userId,
            DateTimeOffset startsAt,
            DateTimeOffset endsAt,
            DateTimeOffset now) : base(id)
        {
            RoomId = roomId;
            UserId = userId;
            StartsAt = startsAt;
            EndsAt = endsAt;
            Status = ReservationStatus.Pending;
            CreatedAt = now;
        }

        public static Reservation Create(
            Guid roomId,
            Guid userId,
            DateTimeOffset startsAt,
            DateTimeOffset endsAt,
            DateTimeOffset now)
        {
            if (roomId == Guid.Empty)
                throw new DomainException("A room is required.");
            if (userId == Guid.Empty)
                throw new DomainException("A user is required.");
            if (startsAt >= endsAt)
                throw new DomainException("The start time must be before the end time.");
            if (startsAt <= now)
                throw new DomainException("A reservation cannot start in the past.");
            var duration = endsAt - startsAt;
            if (duration < MinDuration || duration > MaxDuration)
                throw new DomainException("A reservation must last between 30 minutes and 4 hours.");

            return new Reservation(Guid.NewGuid(), roomId, userId, startsAt, endsAt, now);
        }

        public void Cancel(DateTimeOffset now)
        {
            if (Status == ReservationStatus.Cancelled)
                throw new DomainException("The reservation is already cancelled.");
            if (StartsAt <= now)
                throw new DomainException("A reservation that has already started cannot be cancelled.");
            Status = ReservationStatus.Cancelled;
        }
    }
}
