using System.Collections.Generic;
using CateringBackend.Meals.Commands;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class AddMealCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetValidAddMealCommands()
        {
            yield return new object[]
            {
                new AddMealCommand
                {
                    Name = "testMeal1",
                    Calories = 250,
                    Vegan = true,
                    IngredientList = new string[]
                    {
                        "Kurczak",
                        "Frytki"
                    },
                    AllergenList = new string[]
                    {
                        "Ziemniak",
                        "Orzechy"
                    }
                }
            };

            yield return new object[]
            {
                new AddMealCommand
                {
                    Name = "testMeal2",
                    Calories = 250,
                    Vegan = true,
                    IngredientList = new string[] { },
                    AllergenList = new string[] { }
                }
            };

            yield return new object[]
{
                new AddMealCommand
                {
                    Name = "testMeal2",
                    Calories = 250,
                    Vegan = true
                }
};
        }
    }
}
