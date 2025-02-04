using Bogus;
using CateringBackend.CrossTests.Diets.Requests;
using CateringBackend.CrossTests.Meals;
using CateringBackend.CrossTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CateringBackend.CrossTests.Diets
{
    public static class DietsRequestsProvider
    {
        public static PostDietRequest PreparePostDietRequest(object[] mealIds, bool isValid = true)
        {
            var diet = FakerHelper.GetFaker<PostDietRequest>()
                .RuleFor(x => x.Name, f => f.Lorem.Sentence(5))
                .RuleFor(x => x.Price, f => f.Finance.Amount(1, 1000))
                .RuleFor(x => x.Description, f => f.Lorem.Word())
                .Generate();

            if (isValid)
                diet.MealIds = mealIds?.Select( x => new Guid(x.ToString()))?.ToArray();

            return diet;
        }
    }
}
