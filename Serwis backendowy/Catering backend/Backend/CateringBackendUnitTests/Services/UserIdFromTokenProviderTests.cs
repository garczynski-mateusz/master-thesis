using System;
using System.Security.Claims;
using CateringBackend.AuthUtilities;
using CateringBackend.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace CateringBackendUnitTests.Services
{
    public class UserIdFromTokenProviderTests
    {
        private readonly UserIdFromTokenProvider _userIdFromTokenProvider;

        public UserIdFromTokenProviderTests()
        {
            _userIdFromTokenProvider = new UserIdFromTokenProvider();
        }

        [Fact]
        public void GivenTokenWithoutUserIdClaim_WhenGetUserIdFromContextOrThrow_ThenThrowsJwtTokenDoesNotHaveUserIdClaimException()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();

            // Act & Assert
            Assert.Throws<JwtTokenDoesNotHaveUserIdClaimException>(
                () => _userIdFromTokenProvider.GetUserIdFromContextOrThrow(httpContext.Object));
        }

        [Fact]
        public void GivenUserIdWhichCanNotBeParsed_WhenGetUserIdFromContextOrThrow_ThenThrowsFormatException()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(x => x.User.Claims)
                .Returns(new[] {new Claim(AuthConstants.UserIdClaimType, "IdWhichCanNotBeParsedToGuid")});

            // Act & Assert
            Assert.Throws<FormatException>(
                () => _userIdFromTokenProvider.GetUserIdFromContextOrThrow(httpContext.Object));
        }

        [Fact]
        public void GivenCorrectHttpContextWithUserId_WhenGetUserIdFromContextOrThrow_ThenResultShouldBeEqualToIdInClaim()
        {
            // Arrange
            var userId = new Guid();
            var httpContext = new Mock<HttpContext>();
            httpContext
                .SetupGet(x => x.User.Claims)
                .Returns(new[] { new Claim(AuthConstants.UserIdClaimType, userId.ToString()) });

            // Act
            var result = _userIdFromTokenProvider.GetUserIdFromContextOrThrow(httpContext.Object);

            // Assert
            Assert.Equal(userId, result);
        }
    }
}
