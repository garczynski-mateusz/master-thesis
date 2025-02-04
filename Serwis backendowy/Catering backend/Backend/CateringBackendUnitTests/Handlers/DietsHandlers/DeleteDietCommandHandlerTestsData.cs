using CateringBackend.Diets.Commands;
using CateringBackend.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public class DeleteDietCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetDeleteDietCommandAndDiet()
        {
            var validDietInDatabase = new Diet
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new DeleteDietCommand(validDietInDatabase.Id),
                validDietInDatabase
            };
        }
    }
}
