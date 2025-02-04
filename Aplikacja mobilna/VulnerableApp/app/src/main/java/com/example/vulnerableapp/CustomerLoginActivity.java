package com.example.vulnerableapp;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONObject;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class CustomerLoginActivity extends AppCompatActivity {

    private static final String LOGIN_URL = "https://10.0.2.2:5001/Client/login";
    private OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.customer_login_activity);

        EditText usernameField = findViewById(R.id.email);
        EditText passwordField = findViewById(R.id.password);
        Button loginButton = findViewById(R.id.login_button);
        Button registerButton = findViewById(R.id.register_button);

        loginButton.setOnClickListener(v -> {
            String username = usernameField.getText().toString();
            String password = passwordField.getText().toString();
            try {
                sendCustomerLoginRequest(username, password);
            } catch (Exception e) {
                e.printStackTrace();
                Toast.makeText(CustomerLoginActivity.this, "Error sending request", Toast.LENGTH_SHORT).show();
            }
        });

        registerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Create an Intent to start CustomerRegisterActivity
                Intent intent = new Intent(CustomerLoginActivity.this, CustomerRegisterActivity.class);
                startActivity(intent); // Start the CustomerRegisterActivity
            }
        });
    }

    private void sendCustomerLoginRequest(String username, String password) throws Exception {
        // Create JSON object with email and password
        JSONObject jsonObject = new JSONObject();
        jsonObject.put("email", username);
        jsonObject.put("password", password);

        // Create request body
        MediaType JSON = MediaType.get("application/json; charset=utf-8");
        RequestBody body = RequestBody.create(jsonObject.toString(), JSON);

        // Build the HTTP request
        Request request = new Request.Builder()
                .url(LOGIN_URL)
                .post(body)
                .build();

        // Execute the request asynchronously
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                e.printStackTrace();
                runOnUiThread(() -> Toast.makeText(CustomerLoginActivity.this, "Login failed", Toast.LENGTH_SHORT).show());
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(CustomerLoginActivity.this, "Invalid credentials", Toast.LENGTH_SHORT).show());
                    return;
                }

                String responseBody = response.body().string();
                try {
                    String token = responseBody;

                    // Save the token in SharedPreferences
                    TokenManager.saveToken(CustomerLoginActivity.this, token);

                    runOnUiThread(() -> {
                        Toast.makeText(CustomerLoginActivity.this, "Login successful", Toast.LENGTH_SHORT).show();
                        Intent intent = new Intent(CustomerLoginActivity.this, DietsActivity.class);
                        startActivity(intent);
                    });
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        });
    }
}