using Inventory.Api.Api.Common;
using Inventory.Api.Application.Reservations.CreateReservation;
using Inventory.Api.Application.Reservations.GetReservation;
using Inventory.Api.Application.Reservations.ListReservations;
using Inventory.Api.Application.Reservations.ReleaseReservation;
using MediatR;

namespace Inventory.Api.Api.Reservations;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/reservations")
            .WithTags("Reservations");

        group.MapGet("/", async (
            Guid? stockItemId,
            string? status,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ListReservationsQuery(stockItemId, status), cancellationToken);

            return result.ToHttpResult(reservations => Results.Ok(new ReservationListResponse(reservations)));
        })
        .WithName("ListReservations");

        group.MapPost("/", async (
            CreateReservationRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new CreateReservationCommand(request.StockItemId, request.Quantity, request.ExpiresAtUtc),
                cancellationToken);

            return result.ToHttpResult(reservation =>
                Results.Created($"/reservations/{reservation.Id}", new ReservationResponse(reservation)));
        })
        .WithName("CreateReservation");

        group.MapGet("/{reservationId:guid}", async (
            Guid reservationId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetReservationQuery(reservationId), cancellationToken);

            return result.ToHttpResult(reservation => Results.Ok(new ReservationResponse(reservation)));
        })
        .WithName("GetReservation");

        group.MapPost("/{reservationId:guid}/release", async (
            Guid reservationId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ReleaseReservationCommand(reservationId), cancellationToken);

            return result.ToHttpResult(reservation => Results.Ok(new ReservationResponse(reservation)));
        })
        .WithName("ReleaseReservation");

        return endpoints;
    }
}
