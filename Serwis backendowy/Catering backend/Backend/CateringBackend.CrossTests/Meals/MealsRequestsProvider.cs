using CateringBackend.CrossTests.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CateringBackend.CrossTests.Meals
{
    public static class MealsRequestsProvider
    {
        public static Meal PrepareMeal(bool isValid = true)
        {
            var meal = FakerHelper.GetFaker<Meal>()
                .RuleFor(x => x.MealId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.Name, f => f.Lorem.Sentence(5))
                .RuleFor(x => x.Vegan, f => f.Random.Bool())
                .RuleFor(x => x.AllergenList, f => f.Lorem.Words(5))
                .RuleFor(x => x.IngredientList, f => f.Lorem.Words(5))
                .RuleFor(x => x.Calories, f => f.Random.Number(50, 300))
                .Generate();

            if (!isValid)
                meal.MealId = null;

            return meal;
        }
    }
}
