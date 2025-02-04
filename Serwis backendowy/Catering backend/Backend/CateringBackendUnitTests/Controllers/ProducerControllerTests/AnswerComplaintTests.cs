using CateringBackend.Controllers;
using CateringBackend.Users.Producer.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CateringBackendUnitTests.Controllers.ProducerControllerTests
{
    public class AnswerComplaintTests
    {
        private readonly ProducerController _producersController;
        private readonly Mock<IMediator> _mockedMediator;

        public AnswerComplaintTests()
        {
            _mockedMediator = new Mock<IMediator>();
            _producersController = new ProducerController(_mockedMediator.Object);
        }

        [Fact]
        public async void GivenAnswerComplaintCommand_WhenAnswerComplaint_ThenItIsSentToMediatorWithProperId()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AnswerComplaintWithIdCommand>(), It.IsAny<CancellationToken>()))
                .Verifiable("Answer complaint command was not sent");

            // Act 
            await _producersController.AnswerComplaint(complaintId, new AnswerComplaintCommand());

            // Assert
            _mockedMediator.Verify(x => x.Send(It.IsAny<AnswerComplaintWithIdCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockedMediator.Verify(x => x.Send(It.Is<AnswerComplaintWithIdCommand>(x => x.ComplaintId == complaintId), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(false, false, HttpStatusCode.NotFound)]
        [InlineData(true, false, HttpStatusCode.BadRequest)]
        [InlineData(true, true, HttpStatusCode.Created)]
        public async void GivenMediatorReturnValue_WhenAnswerComplaint_ThenReturnsProperHttpStatusCode(
            bool complaintExists, bool complaintAnswered, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            _mockedMediator
                .Setup(x => x.Send(It.IsAny<AnswerComplaintWithIdCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((complaintExists, complaintAnswered, "")))
                .Verifiable("Answer complaint command was not sent");

            // Act 
            var result = await _producersController.AnswerComplaint(Guid.NewGuid(), new AnswerComplaintCommand());
            var statusCodeActionResult = result as IStatusCodeActionResult;

            // Assert
            Assert.NotNull(statusCodeActionResult);
            Assert.Equal((int)expectedStatusCode, statusCodeActionResult.StatusCode);
        }
    }
}
