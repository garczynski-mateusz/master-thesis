using System;
using System.Collections.Generic;
using System.Linq;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Queries;

namespace CateringBackendUnitTests.Handlers.MealsHandlers
{
    public static class GetMealsQueryHandlerTestsData
    {
        public static IEnumerable<object[]> FilterByNameData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Name = "Meal1", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "Meal2", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "Meal3", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "Meal4", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "UnavailableMeal", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name = "Meal2"
                },
                meals,
                meals.WhereIsAvailable().WhereNameEqual("Meal2").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name = "notExistingMealName"
                },
                meals,
                meals.WhereIsAvailable().WhereNameEqual("notExistingMealName").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name = "UnavailableMeal"
                },
                meals,
                meals.WhereIsAvailable().WhereNameEqual("UnavailableMeal").ToList()
            };
        }

        public static IEnumerable<object[]> FilterByNameWithData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Name = "abcd", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "bcde", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "cdef", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "defg", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "UnavailableMeal", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name_with = "a"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("a").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name_with = "b"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("b").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name_with = "c"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("c").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name_with = "d"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("d").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name = "zxy"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("zxy").ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Name = "UnavailableMeal"
                },
                meals,
                meals.WhereIsAvailable().WhereNameContains("zxy").ToList()
            };
        }

        public static IEnumerable<object[]> FilterByVeganData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), IsVegan = true, IsAvailable = true },
                new() { Id = Guid.NewGuid(), IsVegan = false, IsAvailable = true },
                new() { Id = Guid.NewGuid(), IsVegan = true, IsAvailable = false },
                new() { Id = Guid.NewGuid(), IsVegan = true, IsAvailable = false },
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Vegan = true
                },
                meals,
                meals.WhereIsAvailable().WhereIsVegan().ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Vegan = false
                },
                meals,
                meals.WhereIsAvailable().WhereIsNotVegan().ToList()
            };
        }

        public static IEnumerable<object[]> FilterByCaloriesData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Calories = 10, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 20, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 30, IsAvailable = false },
                new() { Id = Guid.NewGuid(), Calories = 40, IsAvailable = false },
            };

            for (var calories = 0; calories <= 40; calories += 10)
            {
                yield return new object[]
                {
                    new GetMealsQuery
                    {
                        Calories = calories
                    },
                    meals,
                    meals.WhereIsAvailable().WhereCaloriesEqual(calories).ToList()
                };
            }
        }

        public static IEnumerable<object[]> FilterByCaloriesLessThanData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Calories = 10, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 20, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 30, IsAvailable = false },
                new() { Id = Guid.NewGuid(), Calories = 40, IsAvailable = false },
            };

            for (var calories = 0; calories <= 40; calories += 10)
            {
                yield return new object[]
                {
                    new GetMealsQuery
                    {
                        Calories_lt = calories
                    },
                    meals,
                    meals.WhereIsAvailable().WhereCaloriesLessThan(calories).ToList()
                };
            }
        }

        public static IEnumerable<object[]> FilterByCaloriesHigherThanData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Calories = 10, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 20, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 30, IsAvailable = false },
                new() { Id = Guid.NewGuid(), Calories = 40, IsAvailable = false },
            };

            for (var calories = 0; calories <= 40; calories += 10)
            {
                yield return new object[]
                {
                    new GetMealsQuery
                    {
                        Calories_ht = calories
                    },
                    meals,
                    meals.WhereIsAvailable().WhereCaloriesHigherThan(calories).ToList()
                };
            }
        }

        public static IEnumerable<object[]> SortByTitleData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Name = "b", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "a", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "c", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Name = "b", IsAvailable = false },
                new() { Id = Guid.NewGuid(), Name = "d", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Sort = "title(asc)"
                },
                meals,
                meals.WhereIsAvailable().OrderBy(x => x.Name).ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Sort = "title(desc)"
                },
                meals,
                meals.WhereIsAvailable().OrderByDescending(x => x.Name).ToList()
            };
        }

        public static IEnumerable<object[]> SortByCaloriesData()
        {
            var meals = new List<Meal>
            {
                new() { Id = Guid.NewGuid(), Calories = 3, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 2, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 4, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Calories = 7, IsAvailable = false },
                new() { Id = Guid.NewGuid(), Calories = 1, IsAvailable = false },
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Sort = "calories(asc)"
                },
                meals,
                meals.WhereIsAvailable().OrderBy(x => x.Calories).ToList()
            };

            yield return new object[]
            {
                new GetMealsQuery
                {
                    Sort = "calories(desc)"
                },
                meals,
                meals.WhereIsAvailable().OrderByDescending(x => x.Calories).ToList()
            };
        }

        private static IEnumerable<Meal> WhereIsAvailable(this IEnumerable<Meal> meals) => meals.Where(m => m.IsAvailable);

        private static IEnumerable<Meal> WhereNameEqual(this IEnumerable<Meal> meals, string name) => meals.Where(m => m.Name == name);

        private static IEnumerable<Meal> WhereNameContains(this IEnumerable<Meal> meals, string nameWith) => meals.Where(m => m.Name.Contains(nameWith));

        private static IEnumerable<Meal> WhereIsVegan(this IEnumerable<Meal> meals) => meals.Where(m => m.IsVegan);

        private static IEnumerable<Meal> WhereIsNotVegan(this IEnumerable<Meal> meals) => meals.Where(m => !m.IsVegan);

        private static IEnumerable<Meal> WhereCaloriesEqual(this IEnumerable<Meal> meals, int value) => meals.Where(m => m.Calories == value);

        private static IEnumerable<Meal> WhereCaloriesLessThan(this IEnumerable<Meal> meals, int value) => meals.Where(m => m.Calories < value);
        
        private static IEnumerable<Meal> WhereCaloriesHigherThan(this IEnumerable<Meal> meals, int value) => meals.Where(m => m.Calories > value);
    }
}
