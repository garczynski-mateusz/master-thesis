package com.example.vulnerableapp.servermodels;

public class Meal {
    private String id;
    private String name;
    private int calories;
    private boolean isVegan;

    // Getters and Setters
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public int getCalories() { return calories; }
    public void setCalories(int calories) { this.calories = calories; }

    public boolean isVegan() { return isVegan; }
    public void setVegan(boolean isVegan) { this.isVegan = isVegan; }
}