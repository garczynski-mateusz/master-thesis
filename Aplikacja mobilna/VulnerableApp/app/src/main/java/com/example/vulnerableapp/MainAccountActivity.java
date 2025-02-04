package com.example.vulnerableapp;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

import com.example.vulnerableapp.database.DatabaseClient;
import com.example.vulnerableapp.servermodels.Order;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

public class MainAccountActivity extends AppCompatActivity {

    private Button btnAccountDetails, btnOrderHistory;
    private String token;
    private OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_account);

        token = TokenManager.getToken(MainAccountActivity.this);

        // Get data from intent
        Intent intent = getIntent();
        String name = intent.getStringExtra("name");
        String lastName = intent.getStringExtra("lastName");
        String email = intent.getStringExtra("email");
        String phoneNumber = intent.getStringExtra("phoneNumber");
        String street = intent.getStringExtra("street");
        String buildingNumber = intent.getStringExtra("buildingNumber");
        String apartmentNumber = intent.getStringExtra("apartmentNumber");
        String postCode = intent.getStringExtra("postCode");
        String city = intent.getStringExtra("city");

        // Load the default fragment (Account Details)
        CustomerDetailsFragment customerDetailsFragment = CustomerDetailsFragment.newInstance(
                name, lastName, email, phoneNumber, street, buildingNumber, apartmentNumber, postCode, city);
        OrderHistoryFragment orderHistoryFragment = new OrderHistoryFragment();

        fetchOrderHistory(new OnOrdersFetchedCallback() {
            @Override
            public void onSuccess(List<Order> orders) {
                // Handle the orders list
                Executor executor = Executors.newSingleThreadExecutor();
                executor.execute(() -> {
                    updateDatabase(orders);
                });
                orderHistoryFragment.updateOrderList(orders);
            }

            @Override
            public void onError(String errorMessage) {
                Toast.makeText(MainAccountActivity.this, errorMessage, Toast.LENGTH_SHORT).show();
            }
        });

        // Initialize buttons
        btnAccountDetails = findViewById(R.id.btnAccountDetails);
        btnOrderHistory = findViewById(R.id.btnOrderHistory);

        loadFragment(customerDetailsFragment);

        // Set button click listeners
        btnAccountDetails.setOnClickListener(v -> loadFragment(customerDetailsFragment));
        btnOrderHistory.setOnClickListener(v -> loadFragment(orderHistoryFragment));
    }

    private void updateDatabase(List<Order> currentOrders)
    {
        DatabaseClient.getDatabase(this).orderDao().deleteAllOrders();
        for (Order o: currentOrders) DatabaseClient.getDatabase(this).orderDao().insertOrder(o);
    }

    private void loadFragment(Fragment fragment) {
        getSupportFragmentManager()
                .beginTransaction()
                .replace(R.id.fragmentContainer, fragment)
                .commit();
    }

    private void fetchOrderHistory(OnOrdersFetchedCallback callback) {
        String url = "https://10.0.2.2:5001/Client/orders";

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> callback.onError("Error: " + e.getMessage()));
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> callback.onError("Error: " + response.message()));
                    return;
                }

                try {
                    String responseBody = response.body().string();
                    JSONArray ordersArray = new JSONArray(responseBody);
                    List<Order> orders = new ArrayList<>();

                    for (int i = 0; i < ordersArray.length(); i++) {
                        JSONObject orderJson = ordersArray.getJSONObject(i);

                        String id = orderJson.getString("id");
                        String startDate = orderJson.getString("startDate");
                        String endDate = orderJson.getString("endDate");
                        String status = orderJson.getString("status");
                        int price = orderJson.getInt("price");

                        List<Order.Complaint> complaints = new ArrayList<>();
                        JSONArray complaintArray = orderJson.getJSONArray("complaint");

                        for (int j = 0; j < complaintArray.length(); j++) {
                            JSONObject complaintJson = complaintArray.getJSONObject(j);

                            String complaintId = complaintJson.getString("complaintId");
                            String orderId = complaintJson.getString("orderId");
                            String complaintDescription = complaintJson.getString("description");
                            String answer = complaintJson.getString("answer");
                            LocalDateTime date = LocalDateTime.parse(complaintJson.getString("date"));
                            String complaintStatus = complaintJson.getString("status");
                            complaints.add(new Order.Complaint(complaintId, orderId, complaintDescription,
                                    answer, date, complaintStatus));
                        }

                        JSONObject deliveryDetails = orderJson.getJSONObject("deliveryDetails");
                        JSONObject address = deliveryDetails.getJSONObject("address");

                        String street = address.getString("street");
                        String buildingNumber = address.getString("buildingNumber");
                        String apartmentNumber = address.getString("apartmentNumber");
                        String postCode = address.getString("postCode");
                        String city = address.getString("city");

                        String phoneNumber = deliveryDetails.getString("phoneNumber");
                        String commentForDeliverer = deliveryDetails.getString("commentForDeliverer");

                        JSONArray dietsArray = orderJson.getJSONArray("diets");
                        List<Order.Diet> diets = new ArrayList<>();

                        for (int j = 0; j < dietsArray.length(); j++) {
                            JSONObject dietJson = dietsArray.getJSONObject(j);

                            String dietId = dietJson.getString("dietId");
                            String name = dietJson.getString("name");
                            String description = dietJson.getString("description");
                            int dietPrice = dietJson.getInt("price");
                            boolean dietVegan = dietJson.getBoolean("vegan");

                            JSONArray mealsArray = dietJson.getJSONArray("meals");
                            List<Order.Meal> meals = new ArrayList<>();

                            for (int k = 0; k < mealsArray.length(); k++) {
                                JSONObject mealJson = mealsArray.getJSONObject(k);

                                String mealId = mealJson.getString("mealId");
                                String mealName = mealJson.getString("name");
                                int calories = mealJson.getInt("calories");
                                boolean mealVegan = mealJson.getBoolean("vegan");

                                JSONArray ingredientsArray = mealJson.getJSONArray("ingredientList");
                                List<String> ingredientList = new ArrayList<>();
                                for (int m = 0; m < ingredientsArray.length(); m++) {
                                    ingredientList.add(ingredientsArray.getString(m));
                                }

                                JSONArray allergensArray = mealJson.getJSONArray("allergenList");
                                List<String> allergenList = new ArrayList<>();
                                for (int n = 0; n < allergensArray.length(); n++) {
                                    allergenList.add(allergensArray.getString(n));
                                }

                                meals.add(new Order.Meal(mealId, mealName, ingredientList, allergenList, calories, mealVegan));
                            }

                            diets.add(new Order.Diet(dietId, name, description, meals, dietPrice, dietVegan));
                        }

                        orders.add(new Order(id, startDate, endDate, status,
                                new Order.DeliveryDetails(street, buildingNumber, apartmentNumber, postCode, city, phoneNumber, commentForDeliverer),
                                diets, price, complaints));
                    }

                    runOnUiThread(() -> callback.onSuccess(orders));

                } catch (Exception e) {
                    runOnUiThread(() -> callback.onError("Error parsing order history"));
                }
            }
        });
    }

    public interface OnOrdersFetchedCallback {
        void onSuccess(List<Order> orders);
        void onError(String errorMessage);
    }
}
