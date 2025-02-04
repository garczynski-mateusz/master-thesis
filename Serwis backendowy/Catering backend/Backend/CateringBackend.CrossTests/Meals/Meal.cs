namespace CateringBackend.CrossTests.Meals
{
    public class Meal
    {
        public string MealId { get; set; }
        public string Name { get; set; }
        public string[] IngredientList { get; set; }
        public string[] AllergenList { get; set; }
        public int Calories { get; set; }
        public bool Vegan { get; set; }
    }
}
