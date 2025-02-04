package com.example.vulnerableapp.database;

import androidx.room.Database;
import androidx.room.RoomDatabase;
import androidx.room.TypeConverters;

import com.example.vulnerableapp.converters.ComplaintListConverter;
import com.example.vulnerableapp.converters.DietListConverter;
import com.example.vulnerableapp.converters.LocalDateTimeConverter;
import com.example.vulnerableapp.converters.MealListConverter;
import com.example.vulnerableapp.servermodels.Order;

@Database(entities = {Order.class}, version = 1, exportSchema = false)
@TypeConverters({DietListConverter.class, MealListConverter.class, ComplaintListConverter.class, LocalDateTimeConverter.class})
public abstract class AppDatabase extends RoomDatabase {
    public abstract OrderDao orderDao();
}