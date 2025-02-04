package com.example.vulnerableapp;

import android.content.Intent;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class CustomerRegisterActivity extends AppCompatActivity {

    private EditText etName, etLastName, etEmail, etPassword, etPhoneNumber, etStreet, etBuildingNumber, etApartmentNumber, etPostalCode, etCity;
    private Button btnRegister;
    private OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.customer_register_activity);

        // Initialize the views
        etName = findViewById(R.id.etName);
        etLastName = findViewById(R.id.etLastName);
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        etPhoneNumber = findViewById(R.id.etPhoneNumber);
        etStreet = findViewById(R.id.etStreet);
        etBuildingNumber = findViewById(R.id.etBuildingNumber);
        etApartmentNumber = findViewById(R.id.etApartmentNumber);
        etPostalCode = findViewById(R.id.etPostalCode);
        etCity = findViewById(R.id.etCity);
        btnRegister = findViewById(R.id.btnRegister);

        // Add TextWatcher for Postal Code format
        etPostalCode.addTextChangedListener(new TextWatcher() {
            private boolean isUpdating = false;
            private String previousText = "";

            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
                // No action needed before text is changed
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (isUpdating) {
                    return;
                }

                // Remove the '-' and spaces if any exist
                String cleanText = s.toString().replaceAll("-", "");

                if (cleanText.length() > 2) {
                    // Format the postal code: first 2 digits + '-' + remaining digits
                    String formattedText = cleanText.substring(0, 2) + "-" + cleanText.substring(2);
                    isUpdating = true;
                    etPostalCode.setText(formattedText);
                    etPostalCode.setSelection(formattedText.length()); // Move cursor to the end
                    isUpdating = false;
                } else {
                    previousText = cleanText; // Remember the clean text for next check
                }
            }

            @Override
            public void afterTextChanged(Editable s) {
                // No action needed after text is changed
            }
        });

        // Set button click listener
        btnRegister.setOnClickListener(v -> sendRegisterRequest());
    }

    private void sendRegisterRequest() {
        String name = etName.getText().toString();
        String lastName = etLastName.getText().toString();
        String email = etEmail.getText().toString();
        String password = etPassword.getText().toString();
        String phoneNumber = etPhoneNumber.getText().toString();
        String street = etStreet.getText().toString();
        String buildingNumber = etBuildingNumber.getText().toString();
        String apartmentNumber = etApartmentNumber.getText().toString();
        String postalCode = etPostalCode.getText().toString();
        String city = etCity.getText().toString();

        // Create JSON object
        JSONObject jsonObject = new JSONObject();
        try {
            jsonObject.put("name", name);
            jsonObject.put("lastName", lastName);
            jsonObject.put("email", email);
            jsonObject.put("password", password);
            jsonObject.put("phoneNumber", phoneNumber);

            // Create nested address JSON object
            JSONObject addressObject = new JSONObject();
            addressObject.put("street", street);
            addressObject.put("buildingNumber", buildingNumber);
            addressObject.put("apartmentNumber", apartmentNumber);
            addressObject.put("postCode", postalCode);
            addressObject.put("city", city);

            jsonObject.put("address", addressObject);

        } catch (JSONException e) {
            e.printStackTrace();
        }

        // Send POST request
        MediaType JSON = MediaType.get("application/json; charset=utf-8");
        RequestBody body = RequestBody.create(jsonObject.toString(), JSON);
        Request request = new Request.Builder()
                .url("https://10.0.2.2:5001/Client/register")
                .post(body)
                .addHeader("Content-Type", "application/json")
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                e.printStackTrace();
                runOnUiThread(() -> Toast.makeText(CustomerRegisterActivity.this, "Failed to register", Toast.LENGTH_SHORT).show());
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(CustomerRegisterActivity.this, "Registration failed", Toast.LENGTH_SHORT).show());
                } else {
                    String responseBody = response.body().string();
                    try {
                        String token = responseBody;

                        // Save the token in SharedPreferences
                        TokenManager.saveToken(CustomerRegisterActivity.this, token);

                        runOnUiThread(() -> {
                            Toast.makeText(CustomerRegisterActivity.this, "Registration successful", Toast.LENGTH_SHORT).show();
                            // Optionally navigate to another activity
                            Intent intent = new Intent(CustomerRegisterActivity.this, MainActivity.class);
                            startActivity(intent);
                        });
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });
    }
}
