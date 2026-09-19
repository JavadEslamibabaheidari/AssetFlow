using MediatR;

namespace Inventory.Api.Application.Reservations.ExpireReservations;

public sealed record ExpireReservationsCommand(DateTimeOffset? ExpiresBeforeUtc)
    : IRequest<ExpireReservationsDto>;
