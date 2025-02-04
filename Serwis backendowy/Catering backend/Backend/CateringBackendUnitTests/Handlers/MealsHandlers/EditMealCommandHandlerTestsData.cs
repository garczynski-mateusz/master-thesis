using System;
using System.Collections.Generic;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Commands;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public class EditMealCommandHandlerTestsData
    {
        public static IEnumerable<object[]> GetEditMealCommandAndMeal()
        {
            var validMealInDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new EditMealCommand
                {
                    MealId = validMealInDatabase.Id,
                    Name = "NewName",
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
                },
                validMealInDatabase
            };
        }
        public static IEnumerable<object[]> GetEditMealCommandAndMealWithDiets()
        {
            var validMealInDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            var otherMealInDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new EditMealCommand
                {
                    MealId = validMealInDatabase.Id,
                    Name = "NewName",
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
                },
                validMealInDatabase,
                new List<Diet>
                {
                    new Diet
                    {
                        IsAvailable = true,
                        Meals = new HashSet<Meal>() {validMealInDatabase}
                    },
                    new Diet
                    {
                        IsAvailable = true,
                        Meals = new HashSet<Meal>() {otherMealInDatabase}
                    },
                    new Diet
                    {
                        IsAvailable = true,
                        Meals = new HashSet<Meal>() {validMealInDatabase}
                    }
                }
            };
        }
        public static IEnumerable<object[]> GetEditMealCommandAndMealWithDietThatContainsIt()
        {
            var validMealInDatabase = new Meal
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            yield return new object[]
            {
                new EditMealCommand
                {
                    MealId = validMealInDatabase.Id,
                    Name = "NewName",
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
                },
                validMealInDatabase,
                new Diet
                {
                    Id = Guid.NewGuid(),
                    IsAvailable = true,
                    Meals = new HashSet<Meal>() {validMealInDatabase}
                }
            };
        }
    }
}