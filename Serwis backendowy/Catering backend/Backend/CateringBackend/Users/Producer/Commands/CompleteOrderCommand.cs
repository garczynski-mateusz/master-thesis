using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities.Enums;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CateringBackend.Users.Producer.Commands
{
    public record CompleteOrderCommand(Guid orderId): IRequest<(bool orderExists, bool orderCompleted)>;

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, (bool orderExists, bool orderCompleted)>
    {
        private readonly CateringDbContext _dbContext;

        public CompleteOrderCommandHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<(bool orderExists, bool orderCompleted)> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _dbContext.Orders
                .Where(o => o.Status == OrderStatus.ToRealized)
                .FirstOrDefault(o => o.Id == request.orderId);

            if (order == default)
                return (orderExists: false, orderCompleted: false);

            order.Status = OrderStatus.Prepared;
            await _dbContext.SaveChangesAsync();
            return (orderExists: true, orderCompleted: true);
        }
    }
}
