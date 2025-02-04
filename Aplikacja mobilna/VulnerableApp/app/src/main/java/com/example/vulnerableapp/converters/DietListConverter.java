package com.example.vulnerableapp.converters;

import androidx.room.TypeConverter;

import com.example.vulnerableapp.servermodels.Order;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import java.lang.reflect.Type;
import java.util.List;

public class DietListConverter {
    @TypeConverter
    public String fromDietList(List<Order.Diet> diets) {
        return new Gson().toJson(diets);
    }

    @TypeConverter
    public List<Order.Diet> toDietList(String data) {
        Type listType = new TypeToken<List<Order.Diet>>() {}.getType();
        return new Gson().fromJson(data, listType);
    }
}