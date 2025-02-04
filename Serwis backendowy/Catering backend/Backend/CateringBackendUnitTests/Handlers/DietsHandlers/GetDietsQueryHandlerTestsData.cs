using System;
using System.Collections.Generic;
using System.Linq;
using CateringBackend.Diets.Queries;
using CateringBackend.Domain.Entities;
using CateringBackend.Meals.Queries;

namespace CateringBackendUnitTests.Handlers.DietsHandlers
{
    public static class GetDietsQueryHandlerTestsData
    {
        public static IEnumerable<object[]> FilterByNameData()
        {
            var diets = new List<Diet>
            {
                new() { Id = Guid.NewGuid(), Title = "Diet1", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "Diet2", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "Diet3", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "Diet4", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "NotAvailableDiet", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetDietsQuery()
                {
                    Name = "Diet2"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleEqual("Diet2").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name = "notExistingDietName"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleEqual("notExistingDietName").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name = "NotAvailableDiet"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleEqual("NotAvailableDiet").ToList()
            };
        }

        public static IEnumerable<object[]> FilterByNameWithData()
        {
            var diets = new List<Diet>
            {
                new() { Id = Guid.NewGuid(), Title = "abcd", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "bcde", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "cdef", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "defg", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "UnavailableDiet", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name_with = "a"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("a").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name_with = "b"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("b").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name_with = "c"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("c").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name_with = "d"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("d").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name = "zxy"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("zxy").ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Name = "UnavailableMeal"
                },
                diets,
                diets.WhereIsAvailable().WhereTitleContains("zxy").ToList()
            };
        }

        public static IEnumerable<object[]> FilterByVeganData()
        {
            var diets = new List<Diet>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Total vegan diet",
                    Meals = new HashSet<Meal>
                    {
                        new() {IsVegan = true},
                        new() {IsVegan = true},
                        new() {IsVegan = true},
                    },
                    IsAvailable = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Total vegan diet but unavailable",
                    Meals = new HashSet<Meal>
                    {
                        new() {IsVegan = true},
                        new() {IsVegan = true},
                        new() {IsVegan = true},
                    },
                    IsAvailable = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Not really vegan diet",
                    Meals = new HashSet<Meal>
                    {
                        new() {IsVegan = true},
                        new() {IsVegan = true},
                        new() {IsVegan = false},
                    },
                    IsAvailable = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Completely not vegan diet",
                    Meals = new HashSet<Meal>
                    {
                        new() {IsVegan = false},
                        new() {IsVegan = false},
                        new() {IsVegan = false},
                    },
                    IsAvailable = true
                },
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Vegan = true
                },
                diets,
                diets.WhereIsAvailable().WhereIsVegan().ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Vegan = false
                },
                diets,
                diets.WhereIsAvailable().WhereIsNotVegan().ToList()
            };
        }

        public static IEnumerable<object[]> FilterByCaloriesData()
        {
            var diets = new List<Diet>();
            const int from = 0;
            const int to = 40;
            const int step = 5;

            diets.AddRange(GetDietsWithCaloriesRange(from, to, step, available: true));
            diets.AddRange(GetDietsWithCaloriesRange(from, to, step, available: false));

            for (var calories = from; calories <= to; calories += step)
            {
                yield return new object[]
                {
                    new GetDietsQuery()
                    {
                        Calories = calories
                    },
                    diets,
                    diets.WhereIsAvailable().WhereCaloriesEqual(calories).ToList()
                };

                yield return new object[]
                {
                    new GetDietsQuery()
                    {
                        Calories_lt = calories
                    },
                    diets,
                    diets.WhereIsAvailable().WhereCaloriesLessThan(calories).ToList()
                };

                yield return new object[]
                {
                    new GetDietsQuery
                    {
                        Calories_ht = calories
                    },
                    diets,
                    diets.WhereIsAvailable().WhereCaloriesHigherThan(calories).ToList()
                };
            }
        }

        public static IEnumerable<object[]> FilterByPriceData()
        {
            var from = 0;
            var to = 50;
            var step = 5;

            var diets = new List<Diet>();

            for (var price = from; price <= to; price += step)
            {
                diets.Add(new Diet{Id = Guid.NewGuid(), Title = $"Diet with price {price}", Price = price});
            }

            for (var price = from; price <= to; price += step)
            {
                yield return new object[]
                {
                    new GetDietsQuery
                    {
                        Price = price
                    },
                    diets,
                    diets.WhereIsAvailable().WherePriceEqual(price).ToList()
                };

                yield return new object[]
                {
                    new GetDietsQuery
                    {
                        Price_ht = price
                    },
                    diets,
                    diets.WhereIsAvailable().WherePriceHigherThan(price).ToList()
                };

                yield return new object[]
                {
                    new GetDietsQuery
                    {
                        Price_lt = price
                    },
                    diets,
                    diets.WhereIsAvailable().WherePriceLessThan(price).ToList()
                };
            }
        }

        public static IEnumerable<object[]> SortByTitleData()
        {
            var diets = new List<Diet>
            {
                new() { Id = Guid.NewGuid(), Title = "b", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "a", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "c", IsAvailable = true },
                new() { Id = Guid.NewGuid(), Title = "b", IsAvailable = false },
                new() { Id = Guid.NewGuid(), Title = "d", IsAvailable = false },
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "title(asc)"
                },
                diets,
                diets.WhereIsAvailable().OrderBy(x => x.Title).ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "title(desc)"
                },
                diets,
                diets.WhereIsAvailable().OrderByDescending(x => x.Title).ToList()
            };
        }

        public static IEnumerable<object[]> SortByCaloriesData()
        {
            var diets = new List<Diet>
            {
                new() { Id = Guid.NewGuid(), Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(10)), IsAvailable = true },
                new() { Id = Guid.NewGuid(), Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(50)), IsAvailable = true },
                new() { Id = Guid.NewGuid(), Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(40)), IsAvailable = true },
                new() { Id = Guid.NewGuid(), Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(70)), IsAvailable = true },
                new() { Id = Guid.NewGuid(), Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(20)), IsAvailable = true },
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "calories(asc)"
                },
                diets,
                diets.WhereIsAvailable().OrderBy(x => x.Meals.AsEnumerable().Sum(m => m.Calories)).ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "calories(desc)"
                },
                diets,
                diets.WhereIsAvailable().OrderByDescending(x => x.Meals.AsEnumerable().Sum(m => m.Calories)).ToList()
            };
        }

        public static IEnumerable<object[]> SortByPriceData()
        {
            var diets = new List<Diet>
            {
                new() { Id = Guid.NewGuid(), Price = 10, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Price = 50, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Price = 40, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Price = 70, IsAvailable = true },
                new() { Id = Guid.NewGuid(), Price = 20, IsAvailable = true },
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "price(asc)"
                },
                diets,
                diets.WhereIsAvailable().OrderBy(x => x.Price).ToList()
            };

            yield return new object[]
            {
                new GetDietsQuery
                {
                    Sort = "price(desc)"
                },
                diets,
                diets.WhereIsAvailable().OrderByDescending(x => x.Price).ToList()
            };
        }

        private static IEnumerable<Diet> WhereIsAvailable(this IEnumerable<Diet> diets) => diets.Where(d => d.IsAvailable);

        private static IEnumerable<Diet> WhereTitleEqual(this IEnumerable<Diet> diets, string name) => diets.Where(d => d.Title == name);

        private static IEnumerable<Diet> WhereTitleContains(this IEnumerable<Diet> diets, string nameWith) => diets.Where(d => d.Title.Contains(nameWith));

        private static IEnumerable<Diet> WhereIsVegan(this IEnumerable<Diet> diets) => diets.Where(d => d.IsVegan);

        private static IEnumerable<Diet> WhereIsNotVegan(this IEnumerable<Diet> diets) => diets.Where(d => !d.IsVegan);

        private static IEnumerable<Diet> WhereCaloriesEqual(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Calories == value);

        private static IEnumerable<Diet> WhereCaloriesLessThan(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Calories < value);

        private static IEnumerable<Diet> WhereCaloriesHigherThan(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Calories > value);

        private static IEnumerable<Diet> WherePriceEqual(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Price == value);

        private static IEnumerable<Diet> WherePriceLessThan(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Price < value);

        private static IEnumerable<Diet> WherePriceHigherThan(this IEnumerable<Diet> diets, int value) => diets.Where(d => d.Price > value);


        private static IEnumerable<Diet> GetDietsWithCaloriesRange(int from, int to, int step, bool available)
        {
            var generatedDiets = new List<Diet>();
            for (var calories = from; calories < to; calories += step)
            {
                generatedDiets.Add(new Diet
                {
                    Id = Guid.NewGuid(),
                    Title = $"Diet with total {calories} calories",
                    Meals = new HashSet<Meal>(GenerateRandomMealsWithTotalCalories(calories)),
                    IsAvailable = available
                });
            }
            return generatedDiets;
        }

        private static IEnumerable<Meal> GenerateRandomMealsWithTotalCalories(int totalCalories)
        {
            var generatedMeals = new List<Meal>();
            var random = new Random();
            var caloriesLeft = totalCalories;
            while (caloriesLeft > 0)
            {
                var nextMealCalories = random.Next() % caloriesLeft + 1;
                var mealToAdd = new Meal { Name = $"meal with: {nextMealCalories}", Calories = nextMealCalories };
                caloriesLeft -= nextMealCalories;
                generatedMeals.Add(mealToAdd);
            }

            return generatedMeals;
        }

    }
}
