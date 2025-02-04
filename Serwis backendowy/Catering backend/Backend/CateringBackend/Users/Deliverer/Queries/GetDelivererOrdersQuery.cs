using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CateringBackend.Users.Deliverer.Queries
{
    public record GetDelivererOrdersQuery() : IRequest<IEnumerable<DelivererOrdersQueryDTO>>;

    public class DelivererOrdersQueryDTO
    {
        public Guid OrderId { get; set; }
        public DeliveryDetailsDTO DeliveryDetails { get; set; }

        public DelivererOrdersQueryDTO() { }
        public DelivererOrdersQueryDTO(Order o)
        {
            OrderId = o.Id;
            DeliveryDetails = new DeliveryDetailsDTO(o);
        }
    }

    public class GetDelivererOrdersQueryHandler : IRequestHandler<GetDelivererOrdersQuery, IEnumerable<DelivererOrdersQueryDTO>>
    {
        private readonly CateringDbContext _dbContext;

        public GetDelivererOrdersQueryHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DelivererOrdersQueryDTO>> Handle(GetDelivererOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _dbContext.Orders
                .Where(x => x.Status == Domain.Entities.Enums.OrderStatus.Prepared)
                .Include(x => x.Client)
                .ThenInclude(x => x.Address)
                .Select(x => new DelivererOrdersQueryDTO(x))
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
