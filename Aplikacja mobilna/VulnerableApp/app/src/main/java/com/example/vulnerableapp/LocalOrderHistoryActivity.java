package com.example.vulnerableapp;

import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

import com.example.vulnerableapp.database.DatabaseClient;
import com.example.vulnerableapp.servermodels.Order;
import java.util.List;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public class LocalOrderHistoryActivity  extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_local_order_history);

        // Load the default fragment (Account Details)
        OrderHistoryFragment orderHistoryFragment = new OrderHistoryFragment();

        Executor executor = Executors.newSingleThreadExecutor();
        executor.execute(() -> {
            List<Order> orders = DatabaseClient.getDatabase(this).orderDao().getAllOrders();
            // Switch to the main thread to update the UI
            runOnUiThread(() -> {
                orderHistoryFragment.updateOrderList(orders);
            });
        });

        loadFragment(orderHistoryFragment);
        /*// Load the default fragment (Account Details)
        OrderHistoryFragment orderHistoryFragment = new OrderHistoryFragment();
        loadFragment(orderHistoryFragment);

        // This runs on the main thread automatically
        DatabaseClient.getDatabase(this).orderDao().getAllOrders()
                .observe(this, orderHistoryFragment::updateOrderList);*/
    }

    private void loadFragment(Fragment fragment) {
        getSupportFragmentManager()
                .beginTransaction()
                .replace(R.id.fragmentContainer, fragment)
                .commit();
    }
}