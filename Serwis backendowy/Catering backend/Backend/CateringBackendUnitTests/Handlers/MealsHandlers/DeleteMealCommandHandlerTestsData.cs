using System;
using System.Collections.Generic;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Commands;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class DeleteMealCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetDeleteMealCommandAndMeal()
        {
            var validMealInDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new DeleteMealCommand(validMealInDatabase.Id),
                validMealInDatabase
            };
        }
    }
}