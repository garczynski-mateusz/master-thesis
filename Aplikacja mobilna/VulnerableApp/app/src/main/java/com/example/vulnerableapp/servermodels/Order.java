package com.example.vulnerableapp.servermodels;

import androidx.annotation.NonNull;
import androidx.room.Entity;
import androidx.room.PrimaryKey;
import androidx.room.Embedded;
import androidx.room.TypeConverters;

import com.example.vulnerableapp.converters.ComplaintListConverter;
import com.example.vulnerableapp.converters.DietListConverter;
import com.example.vulnerableapp.converters.LocalDateTimeConverter;
import com.example.vulnerableapp.converters.MealListConverter;

import java.time.LocalDateTime;
import java.util.List;


@Entity(tableName = "orders")
public class Order {
    @PrimaryKey @NonNull
    public String id;
    @TypeConverters(DietListConverter.class) // Custom converter for lists
    public List<Diet> diets;
    @Embedded
    public DeliveryDetails deliveryDetails;
    public String startDate;
    public String endDate;
    public int price;
    public String status;
    @TypeConverters(ComplaintListConverter.class)
    public List<Complaint> complaints;

    public Order() {}

    public Order(String id, String startDate, String endDate, String status,
                 DeliveryDetails deliveryDetails, List<Diet> diets, int price, List<Complaint> complaints)
    {
        this.id = id;
        this.diets = diets;
        this.deliveryDetails = deliveryDetails;
        this.startDate = startDate;
        this.endDate = endDate;
        this.price = price;
        this.status = status;
        this.complaints = complaints;
    }

    public static class Diet {
        public String dietId;
        public String name;
        public String description;
        @TypeConverters(MealListConverter.class)
        public List<Meal> meals;
        public int price;
        public boolean vegan;

        public Diet() {}

        public Diet(String dietId, String name, String description,
                    List<Meal> meals, int price, boolean vegan)
        {
            this.dietId = dietId;
            this.name = name;
            this.description = description;
            this.meals = meals;
            this.price = price;
            this.vegan = vegan;
        }
    }

    public static class Meal {
        public String mealId;
        public String name;
        public List<String> ingredientList;
        public List<String> allergenList;
        public int calories;
        public boolean vegan;

        public Meal() {}

        public Meal(String mealId, String name, List<String> ingredientList,
                    List<String> allergenList, int calories, boolean vegan)
        {
            this.mealId = mealId;
            this.name = name;
            this.ingredientList = ingredientList;
            this.allergenList = allergenList;
            this.calories = calories;
            this.vegan = vegan;
        }
    }

    public static class DeliveryDetails {
        @Embedded
        public Address address;
        public String phoneNumber;
        public String commentForDeliverer;

        public DeliveryDetails() {}

        public DeliveryDetails(Address address, String phoneNumber, String commentForDeliverer)
        {
            this.address = address;
            this.phoneNumber = phoneNumber;
            this.commentForDeliverer = commentForDeliverer;
        }

        public DeliveryDetails(String Street, String BuildingNumber, String ApartmentNumber,
                               String PostCode, String City, String PhoneNumber, String CommentForDeliverer)
        {
            this.address = new Address(Street, BuildingNumber, ApartmentNumber, PostCode, City);
            this.phoneNumber = PhoneNumber;
            this.commentForDeliverer = CommentForDeliverer;
        }

        public static class Address {
            public String street;
            public String buildingNumber;
            public String apartmentNumber;
            public String postCode;
            public String city;

            public Address(String street, String buildingNumber, String apartmentNumber,
                           String postCode, String city)
            {
                this.street = street;
                this.buildingNumber = buildingNumber;
                this.apartmentNumber = apartmentNumber;
                this.postCode = postCode;
                this.city = city;
            }
        }
    }

    public static class Complaint {
        public String complaintId;
        public String orderId;
        public String description;
        public String answer;
        @TypeConverters(LocalDateTimeConverter.class)
        public LocalDateTime date;
        public String status;

        public Complaint() {}

        public Complaint(String complaintId, String orderId, String description, String answer,
                         LocalDateTime date, String status)
        {
            this.complaintId = complaintId;
            this.orderId = orderId;
            this.description = description;
            this.answer = answer;
            this.date = date;
            this.status = status;
        }
    }
}

