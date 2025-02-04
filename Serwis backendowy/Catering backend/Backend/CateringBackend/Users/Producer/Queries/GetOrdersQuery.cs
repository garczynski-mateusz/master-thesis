using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Search;
using CateringBackend.Users.Client.Queries;
using CateringBackend.Utilities.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CateringBackend.Users.Producer.Queries
{
    public class GetOrdersQuery : BaseSearchQuery<Order>, IRequest<IEnumerable<OrderDTO>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        protected override IQueryable<Order> GetFiltered(IQueryable<Order> collectionToFilter)
            => collectionToFilter
                .FilterIf(o => o.StartDate.Day == StartDate.Value.Day &&
                               o.StartDate.Month == StartDate.Value.Month &&
                               o.StartDate.Year == StartDate.Value.Year, StartDate != null)
                .FilterIf(o => o.EndDate.Day == EndDate.Value.Day &&
                               o.EndDate.Month == EndDate.Value.Month &&
                               o.EndDate.Year == EndDate.Value.Year, EndDate != null);

        protected override IQueryable<Order> GetSorted(IQueryable<Order> collectionToSort)
            => Sort switch
            {
                null => collectionToSort,
                "startDate(asc)" => collectionToSort.Sort(o => o.StartDate, true),
                "startDate(desc)" => collectionToSort.Sort(o => o.StartDate, false),
                "endDate(asc)" => collectionToSort.Sort(o => o.EndDate, true),
                "endDate(desc)" => collectionToSort.Sort(o => o.EndDate, false),
                "orderId(asc)" => collectionToSort.Sort(o => o.Id, true),
                "orderId(desc)" => collectionToSort.Sort(o => o.Id, false),
                _ => throw new ArgumentOutOfRangeException(nameof(Sort))
            };
    }

    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly CateringDbContext _dbContext;

        public GetOrdersQueryHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders =  _dbContext.Orders
                .Where(o => o.Status == OrderStatus.ToRealized)
                .Include(o => o.Diets)
                .ThenInclude(d => d.Meals)
                .Include(o => o.Client)
                .ThenInclude(c => c.Address);

            var searchResult = await request.GetSearchResult(orders);

            return searchResult.Select(o => new OrderDTO(o)).ToList();
        }
    }
}
