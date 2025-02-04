package com.example.vulnerableapp;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.adapters.CartAdapter;
import com.example.vulnerableapp.servermodels.Diet;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.Calendar;
import java.util.Date;
import java.text.SimpleDateFormat;
import java.util.TimeZone;
import java.util.Map;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class CartActivity extends AppCompatActivity {

    private RecyclerView recyclerCartItems;
    private DatePicker datePickerStart, datePickerEnd;
    private TextView tvDateError, tvTotalPrice;
    private EditText etAdditionalInfo;
    private Button btnOrder;
    private JSONObject addressForNotification;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_cart);

        // Initialize views
        recyclerCartItems = findViewById(R.id.recyclerCartItems);
        datePickerStart = findViewById(R.id.datePickerStart);
        datePickerEnd = findViewById(R.id.datePickerEnd);
        tvDateError = findViewById(R.id.tvDateError);
        tvTotalPrice = findViewById(R.id.tvTotalPrice);
        etAdditionalInfo = findViewById(R.id.etAdditionalInfo);
        btnOrder = findViewById(R.id.btnOrder);

        // Set up RecyclerView
        recyclerCartItems.setLayoutManager(new LinearLayoutManager(this));
        recyclerCartItems.setAdapter(new CartAdapter(CartManager.getInstance().getCart()));

        // Date validation and listeners for date pickers
        Calendar today = Calendar.getInstance();
        today.add(Calendar.DAY_OF_YEAR, 1); // Adding one day
        long tomorrow = today.getTimeInMillis();
        datePickerStart.setMinDate(tomorrow);
        setDatePickerLimit(datePickerStart, today);
        setDatePickerLimit(datePickerEnd, today);

        // Set DatePicker listeners to recalculate total price when dates change
        datePickerStart.setOnDateChangedListener(new DatePicker.OnDateChangedListener() {
            @Override
            public void onDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                calculateTotalPrice();
            }
        });

        datePickerEnd.setOnDateChangedListener(new DatePicker.OnDateChangedListener() {
            @Override
            public void onDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                calculateTotalPrice();
            }
        });

        btnOrder.setOnClickListener(v -> validateAndPlaceOrder());
    }

    // Method to set minimum date for date pickers
    private void setDatePickerLimit(@NonNull DatePicker datePicker, Calendar minDate) {
        datePicker.setMinDate(minDate.getTimeInMillis());
    }

    private void validateAndPlaceOrder() {
        if (validateDates()) {
            getClientInfo();
        }
    }

    private void getClientInfo() {
        String url = "https://10.0.2.2:5001/Client/account"; // Replace with your actual URL
        String token = TokenManager.getToken(CartActivity.this);

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token) // Add Bearer token
                .get()
                .build();

        // Execute the request
        OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> Toast.makeText(CartActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show());
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(CartActivity.this, "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                    return;
                }

                String responseData = response.body().string();
                try {
                    JSONObject jsonResponse = new JSONObject(responseData);
                    // Extract address and phone number
                    JSONObject address = jsonResponse.getJSONObject("address");
                    String phoneNumber = jsonResponse.getString("phoneNumber");

                    // Prepare order data
                    createOrder(address, phoneNumber);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }
        });
    }

    private void createOrder(JSONObject address, String phoneNumber) {
        // Prepare the diet IDs from CartManager
        addressForNotification = address;
        Map<Diet, Integer> cart = CartManager.getInstance().getCart();
        JSONArray dietIDs = new JSONArray();
        for (Diet diet : cart.keySet()) {
            dietIDs.put(diet.getId());
        }

        // Prepare the order JSON object
        JSONObject deliveryDetails = new JSONObject();
        try {
            JSONObject deliveryAddress = new JSONObject();
            deliveryAddress.put("street", address.getString("street"));
            deliveryAddress.put("buildingNumber", address.getString("buildingNumber"));
            deliveryAddress.put("apartmentNumber", address.getString("apartmentNumber"));
            deliveryAddress.put("postCode", address.getString("postCode"));
            deliveryAddress.put("city", address.getString("city"));

            deliveryDetails.put("address", deliveryAddress);
            deliveryDetails.put("phoneNumber", phoneNumber);
            deliveryDetails.put("commentForDeliverer", etAdditionalInfo.getText().toString());

            JSONObject orderData = new JSONObject();
            orderData.put("dietIDs", dietIDs);
            orderData.put("deliveryDetails", deliveryDetails);
            orderData.put("startDate", getStartDate());
            orderData.put("endDate", getEndDate());

            // Make the POST call
            postOrder(orderData);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void postOrder(JSONObject orderData) {
        String url = "https://10.0.2.2:5001/Client/orders";
        String token = TokenManager.getToken(CartActivity.this);

        RequestBody body = RequestBody.create(orderData.toString(), MediaType.parse("application/json"));

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token) // Add Bearer token
                .post(body)
                .build();

        OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> Toast.makeText(CartActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show());
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(CartActivity.this, "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                    return;
                }

                String orderId = response.body().string().trim();

                navigateToNextScreen(orderId);
            }
        });
    }

    // Validate the selected dates
    private boolean validateDates() {
        Calendar startDate = Calendar.getInstance();
        startDate.set(datePickerStart.getYear(), datePickerStart.getMonth(), datePickerStart.getDayOfMonth());

        Calendar endDate = Calendar.getInstance();
        endDate.set(datePickerEnd.getYear(), datePickerEnd.getMonth(), datePickerEnd.getDayOfMonth());

        Date currentDate = new Date();

        // Validation logic
        if (startDate.getTime().before(currentDate)) {
            tvDateError.setText("Start date cannot be before today.");
            tvDateError.setVisibility(TextView.VISIBLE);
            return false;
        } else if (endDate.before(startDate)) {
            tvDateError.setText("End date cannot be before start date.");
            tvDateError.setVisibility(TextView.VISIBLE);
            return false;
        } else {
            tvDateError.setVisibility(TextView.GONE); // Hide error if validation passes
            return true;
        }
    }

    private void calculateTotalPrice() {
        // Assuming dietList contains the diets in the cart
        double totalDietPrice = 0;
        Map<Diet, Integer> cart = CartManager.getInstance().getCart();

        for (Map.Entry<Diet, Integer> entry : cart.entrySet()) {
            Diet diet = entry.getKey();
            int quantity = entry.getValue(); // Get the quantity of the diet

            totalDietPrice += diet.getPrice() * quantity; // Assuming getPrice() returns the price of a diet
        }

        // Calculate number of days between the selected dates
        Calendar startCalendar = Calendar.getInstance();
        startCalendar.set(datePickerStart.getYear(), datePickerStart.getMonth(), datePickerStart.getDayOfMonth());

        Calendar endCalendar = Calendar.getInstance();
        endCalendar.set(datePickerEnd.getYear(), datePickerEnd.getMonth(), datePickerEnd.getDayOfMonth());

        long diffInMillis = endCalendar.getTimeInMillis() - startCalendar.getTimeInMillis();
        long daysBetween = diffInMillis / (1000 * 60 * 60 * 24);

        if (daysBetween < 0) {
            daysBetween = 0; // Handle case where end date is before start date
        }

        daysBetween++;

        // Calculate total price
        int totalPrice = (int) (totalDietPrice * (int) daysBetween);

        // Update total price TextView
        tvTotalPrice.setText("Total Price: " + totalPrice);
    }

    private String getStartDate() {
        Calendar startCalendar = Calendar.getInstance();
        startCalendar.set(datePickerStart.getYear(), datePickerStart.getMonth(), datePickerStart.getDayOfMonth());

        // Create a SimpleDateFormat for the desired format
        SimpleDateFormat isoFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        isoFormat.setTimeZone(TimeZone.getTimeZone("UTC")); // Set timezone to UTC

        // Format the date and return it
        return isoFormat.format(startCalendar.getTime());
    }

    private String getEndDate() {
        Calendar endCalendar = Calendar.getInstance();
        endCalendar.set(datePickerEnd.getYear(), datePickerEnd.getMonth(), datePickerEnd.getDayOfMonth());

        // Create a SimpleDateFormat for the desired format
        SimpleDateFormat isoFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        isoFormat.setTimeZone(TimeZone.getTimeZone("UTC")); // Set timezone to UTC

        // Format the date and return it
        return isoFormat.format(endCalendar.getTime());
    }

    private void navigateToNextScreen(String orderId) {
        CartManager.getInstance().clearCart();
        runOnUiThread(() -> Toast.makeText(this, "Order placed successfully!", Toast.LENGTH_SHORT).show());

        String address;
        try {
            address = addressForNotification.getString("street")
                    + " " + addressForNotification.getString("buildingNumber")
                    + " " + addressForNotification.getString("apartmentNumber")
                    + " " + addressForNotification.getString("postCode")
                    + " " + addressForNotification.getString("city");
        } catch (JSONException e) {
            address = "Unknown";
        }

        NotificationHelper.scheduleNotification(this, "Explicit BroadcastReceiver",
                "Your order is ready to pick up at " + address + "!");
        //ImplicitNotificationHelper.scheduleNotification(this, "Implicit BroadcastReceiver",
        //        "Your order is ready to pick up at " + address + "!");


        startActivity(new Intent(this, SummaryActivity.class)
                .putExtra("ORDER_ID", orderId)
                .addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK));
        finish(); // Finish the current activity to remove it from the stack
    }
}
