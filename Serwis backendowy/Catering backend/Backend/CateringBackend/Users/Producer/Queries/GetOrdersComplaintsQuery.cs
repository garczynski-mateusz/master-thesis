using CateringBackend.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CateringBackend.Users.Client.Queries.OrderDTO;

namespace CateringBackend.Users.Producer.Queries
{
    public record GetOrdersComplaintsQuery : IRequest<IEnumerable<ComplaintDTO>>;

    public class GetOrdersComplaintsQueryHandler : IRequestHandler<GetOrdersComplaintsQuery, IEnumerable<ComplaintDTO>>
    {
        private readonly CateringDbContext _dbContext;

        public GetOrdersComplaintsQueryHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ComplaintDTO>> Handle(GetOrdersComplaintsQuery request, CancellationToken cancellationToken) =>
            await _dbContext.Complaints.Select(c => new ComplaintDTO(c)).ToListAsync();
    }
}
