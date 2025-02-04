package com.example.vulnerableapp;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class SummaryActivity extends AppCompatActivity {

    private TextView tvOrderId;
    private Button btnPay;
    private Button btnBack;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_summary);

        // Initialize UI elements
        tvOrderId = findViewById(R.id.tvOrderId);
        btnPay = findViewById(R.id.btnPay);
        btnBack = findViewById(R.id.btnBack);

        // Retrieve the order ID from the intent
        String orderId = getIntent().getStringExtra("ORDER_ID");
        tvOrderId.setText("Your order id: " + orderId); // Set the order ID text

        // Set button listeners
        btnPay.setOnClickListener(v -> {
            processPayment(orderId);
        });

        btnBack.setOnClickListener(v -> {
            // Navigate back to DietsActivity
            Intent intent = new Intent(SummaryActivity.this, DietsActivity.class);
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK); // Clear the stack
            startActivity(intent); // Start the DietsActivity
            finish(); // Finish this activity
            finish(); // Finish the previous DietsActivity
        });
    }

    private void processPayment(String orderId) {
        // Prepare the POST request
        String url = "https://10.0.2.2:5001/Client/orders/" + orderId + "/pay";
        String token = TokenManager.getToken(SummaryActivity.this); // Get the Bearer token

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .post(RequestBody.create("", MediaType.parse("application/json"))) // Empty body
                .build();

        // Use OkHttpClient for async call
        OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient(); // Use your unsafe OkHttp client setup
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> Toast.makeText(SummaryActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show());
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(SummaryActivity.this, "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                    return;
                }

                runOnUiThread(() -> {
                    // Show success message
                    Toast.makeText(SummaryActivity.this, "You have paid!", Toast.LENGTH_SHORT).show();

                    // Navigate back to DietsActivity
                    Intent intent = new Intent(SummaryActivity.this, DietsActivity.class);
                    intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK); // Clear the stack
                    startActivity(intent);
                    finish(); // Finish this activity
                    finish(); // Finish the previous DietsActivity
                });
            }
        });
    }
}
