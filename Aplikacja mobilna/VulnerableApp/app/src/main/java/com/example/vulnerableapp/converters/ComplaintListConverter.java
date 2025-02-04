package com.example.vulnerableapp.converters;

import androidx.room.TypeConverter;

import com.example.vulnerableapp.servermodels.Order;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import java.lang.reflect.Type;
import java.util.List;

public class ComplaintListConverter {
    @TypeConverter
    public String fromComplaintList(List<Order.Complaint> complaints) {
        return new Gson().toJson(complaints);
    }

    @TypeConverter
    public List<Order.Complaint> toComplaintList(String data) {
        Type listType = new TypeToken<List<Order.Complaint>>() {}.getType();
        return new Gson().fromJson(data, listType);
    }
}