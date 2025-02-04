using CateringBackend.Domain.Entities;
using CateringBackend.Domain.Entities.Enums;
using CateringBackend.Users.Client.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CateringBackendUnitTests.Handlers.ClientHandlers
{
    public static class GetOrdersWithUserIdQueryHandlerTestsData
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
                        Status = OrderStatus.WaitingForPayment,
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
                        Status = OrderStatus.Finished,
                        ClientId = clientToAddToDb.Id
                    }
                },

                new GetOrdersQueryWithUserId
                {
                    UserId = clientToAddToDb.Id
                },

                clientToAddToDb
            };
        }

        public static IEnumerable<object[]> FilterByStartDateData()
        {
            var client = new Client { Id = Guid.NewGuid() };
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(-2)},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(-1)},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(1)},
                new() { Id = Guid.NewGuid(), ClientId = client.Id, StartDate = DateTime.Today.AddDays(2)},
            };

            yield return new object[]
            {
                new GetOrdersQueryWithUserId
                {
                    UserId = client.Id,
                    StartDate = DateTime.Today
                },
                orders,
                orders.Where(o => o.StartDate == DateTime.Today).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQueryWithUserId
                {
                    UserId = client.Id,
                    StartDate = DateTime.Today.AddDays(-1)
                },
                orders,
                orders.Where(o => o.StartDate == DateTime.Today.AddDays(-1)).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQueryWithUserId
                {
                    UserId = client.Id,
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
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 150 },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 250 },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 350 },
                new() {Id = Guid.NewGuid(), ClientId = client.Id, Price = 450 }
            };

            yield return new object[]
            {
                new GetOrdersQueryWithUserId
                {
                    UserId = client.Id,
                    Sort = "price(asc)"
                },
                orders,
                orders.OrderBy(o => o.Price).ToList(),
                client
            };

            yield return new object[]
            {
                new GetOrdersQueryWithUserId
                {
                    UserId = client.Id,
                    Sort = "price(desc)"
                },
                orders,
                orders.OrderByDescending(o => o.Price).ToList(),
                client
            };
        }
    }
}
