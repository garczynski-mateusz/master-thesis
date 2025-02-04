package com.example.vulnerableapp.database;

import androidx.lifecycle.LiveData;
import androidx.room.Dao;
import androidx.room.Insert;
import androidx.room.Query;

import com.example.vulnerableapp.servermodels.Order;

import java.util.List;

@Dao
public interface OrderDao {
    @Insert
    void insertOrder(Order order);

    @Query("SELECT * FROM orders WHERE id = :orderId")
    Order getOrderById(String orderId);

    @Query("SELECT * FROM orders")
    List<Order> getAllOrders();

    //@Query("SELECT * FROM orders")
    //LiveData<List<Order>> getAllOrders();

    @Query("DELETE FROM orders WHERE id = :orderId")
    void deleteOrder(String orderId);

    @Query("DELETE FROM orders")
    void deleteAllOrders();
}