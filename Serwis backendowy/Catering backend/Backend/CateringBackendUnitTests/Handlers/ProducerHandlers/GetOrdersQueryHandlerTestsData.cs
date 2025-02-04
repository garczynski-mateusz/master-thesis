using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Producer.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CateringBackendUnitTests.Handlers.ProducerHandlers
{
    public static class GetOrdersQueryHandlerTestsData
    {
        public static IEnumerable<object[]> GetEmptyOrdresWithEmptyGetOrdersQuery()
        {
            var clientToAddToDb = new Client
            {
                Id = Guid.NewGuid()
            };
            yield return new object[]
            {
                new List<Order>
                {
                    new Order
                    {
                        Id = Guid.NewGuid(),
                        Status = OrderStatus.ToRealized,
                        ClientId = clientToAddToDb.Id

                    },
                    new Order
                    {
                        Id = Guid.NewGuid(),
                        Status = OrderStatus.Paid,
                        ClientId = clientToAddToDb.Id
                    },
                    new Order
                    {
                        Id = Guid.NewGuid(),
                        Status = OrderStatus.ToRealized,
                        ClientId = clientToAddToDb.Id
                    }
                },

                new GetOrdersQuery(),

                clientToAddToDb
            };
        }

        public static IEnumerable<object[]> FilterByStartDateData()
        {
            var client = new Client { Id = Guid.NewGuid() };
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(-2), Status = OrderStatus.ToRealized},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(-1), Status = OrderStatus.ToRealized},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today, Status = OrderStatus.ToRealized},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(1), Status = OrderStatus.ToRealized},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(2), Status = OrderStatus.ToRealized},
            };

            yield return new object[]
            {
                new GetOrdersQuery
                {
                    StartDate = DateTime.Today
                },
                orders,
                orders.Where(o => o.StartDate == DateTime.Today).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQuery
                {
                    StartDate = DateTime.Today.AddDays(-1)
                },
                orders,
                orders.Where(o => o.StartDate == DateTime.Today.AddDays(-1)).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQuery
                {
                    StartDate = DateTime.Today.AddDays(-3)
                },
                orders,
                orders.Where(o => o.StartDate == DateTime.Today.AddDays(-3)).ToList(),
                client
            };
        }

        public static IEnumerable<object[]> SortByPriceData()
        {
            var client = new Client { Id = Guid.NewGuid() };
            var orders = new List<Order>
            {
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 150, Status = OrderStatus.ToRealized },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 250, Status = OrderStatus.ToRealized },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 350, Status = OrderStatus.ToRealized },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 450, Status = OrderStatus.ToRealized }
            };

            yield return new object[]
            {
                new GetOrdersQuery
                {
                    Sort = "price(asc)"
                },
                orders,
                orders.OrderBy(o => o.Price).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQuery
                {
                    Sort = "price(desc)"
                },
                orders,
                orders.OrderByDescending(o => o.Price).ToList(),
                client
            };
        }
    }
}
