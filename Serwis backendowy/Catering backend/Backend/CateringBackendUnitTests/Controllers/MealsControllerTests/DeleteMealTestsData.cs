using System.Collections.Generic;
using System.Net;
using CateringBackend.Domain.Entities;

namespace CateringBackendUnitTests.Controllers.MealsControllerTests
{
    public class DeleteMealTestsData
    {
        public static IEnumerable<object[]> GetPossibleMediatorReturnsWithExpectedResultCode()
        {
            yield return new object[]
            {
                new Meal
                {
                    IsAvailable = true
                }, 
                HttpStatusCode.BadRequest
            };

            yield return new object[]
            {
                new Meal
                {
                    IsAvailable = false
                },
                HttpStatusCode.OK
            };

            yield return new object[]
            {
                null,
                HttpStatusCode.NotFound
            };
        }
    }
}