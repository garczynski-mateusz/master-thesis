using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.Controllers;
using CateringBackend.Diets;
using CateringBackend.Diets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Controllers.DietsControllerTests
{
    public class GetDietsTests
    {
        private readonly DietsController _dietsController;
        private readonly Mock<IMediator> _mockedMediator;

        public GetDietsTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _dietsController = new DietsController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenGetDietsQuery_WhenGetDiets_ThenItIsSentToMediator()
        {
            // Arrange
            var getDietsQuery = new GetDietsQuery();

            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetDietsQuery>(), It.IsAny<CancellationToken>()))
                .Verifiable("GetDietsQuery was not sent");

            // Act
            await _dietsController.GetDiets(getDietsQuery);

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<GetDietsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void GivenMediatorResult_WhenGetDiets_ThenReturnsOK()
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<GetDietsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<DietsSearchResultDTO>>(new List<DietsSearchResultDTO>()))
                .Verifiable("GetDietsQuery was not sent");

            // Act 
            var result = await _dietsController.GetDiets(new GetDietsQuery());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeActionResult.StatusCode);
        }
    }
}
