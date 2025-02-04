package com.example.vulnerableapp.servermodels;

import java.util.List;

public class Diet {
    private String id;
    private String name;
    private int price;
    private int calories;
    private boolean vegan;
    private boolean isExpanded;
    //private String mealsInfo;
    private List<Meal> meals;

    // Getters and Setters
    public boolean isExpanded() { return isExpanded; }
    public void setExpanded(boolean expanded) { isExpanded = expanded; }

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public int getPrice() { return price; }
    public void setPrice(int price) { this.price = price; }

    public int getCalories() { return calories; }
    public void setCalories(int calories) { this.calories = calories; }

    public boolean isVegan() { return vegan; }
    public void setVegan(boolean vegan) { this.vegan = vegan; }

    //public String getMealsInfo() { return mealsInfo; }
    //public void setMealsInfo(String mealsInfo) { this.mealsInfo = mealsInfo; }

    public List<Meal> getMeals() { return meals; }
    public void setMeals(List<Meal> meals) { this.meals = meals; }
}