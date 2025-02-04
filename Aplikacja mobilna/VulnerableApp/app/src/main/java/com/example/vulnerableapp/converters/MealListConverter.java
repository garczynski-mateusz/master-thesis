package com.example.vulnerableapp.converters;

import androidx.room.TypeConverter;

import com.example.vulnerableapp.servermodels.Order;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import java.lang.reflect.Type;
import java.util.List;

public class MealListConverter {
    @TypeConverter
    public String fromMealList(List<Order.Meal> meals) {
        return new Gson().toJson(meals);
    }

    @TypeConverter
    public List<Order.Meal> toMealList(String data) {
        Type listType = new TypeToken<List<Order.Meal>>() {}.getType();
        return new Gson().fromJson(data, listType);
    }
}