using CateringBackend.Controllers;
using CateringBackend.Users.Producer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static CateringBackend.Users.Client.Queries.OrderDTO;

namespace CateringBackendUnitTests.Controllers.ProducerControllerTests
{
    public class GetOrderComplaintsTests
    {
        private readonly ProducerController _producerController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetOrderComplaintsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _producerController = new ProducerController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenGetOrdersComplaintsQuery_ThenItIsSentToMediator()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersComplaintsQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("Get orders complaints query was not sent");

            // Act 
            await _producerController.GetOrdersComplaints();

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<GetOrdersComplaintsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenGetOrdersComplaintsQuery_WhenGetOrdersComplaints_ThenReturnHtppOkStatusCode()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetOrdersComplaintsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IEnumerable<ComplaintDTO>)null))
                .Verifiable("Get orders complaints query was not sent");

            // Act 
            var result = await _producerController.GetOrdersComplaints();
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult.StatusCode);
        }
    }
}
