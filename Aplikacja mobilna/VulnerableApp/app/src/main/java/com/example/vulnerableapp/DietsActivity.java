package com.example.vulnerableapp;

import com.example.vulnerableapp.adapters.DietAdapter;
import com.example.vulnerableapp.servermodels.Diet;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

public class DietsActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private DietAdapter adapter;
    private List<Diet> dietList = new ArrayList<>();
    private ProgressBar progressBar;
    private Button btnNext, btnPrev;
    private ImageButton btnCart, btnLogout, btnAccountDetails;

    private int offset = 0;
    private static final int LIMIT = 10;

    private OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient();
    private String token;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_diets);

        recyclerView = findViewById(R.id.recyclerViewDiets);
        progressBar = findViewById(R.id.progressBar);
        btnNext = findViewById(R.id.btnNext);
        btnPrev = findViewById(R.id.btnPrev);
        token = TokenManager.getToken(DietsActivity.this);
        btnLogout = findViewById(R.id.btnLogout);
        btnAccountDetails = findViewById(R.id.btnAccountDetails);
        btnCart = findViewById(R.id.btnCart);

        adapter = new DietAdapter(dietList, this, token);
        recyclerView.setAdapter(adapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        btnNext.setOnClickListener(v -> {
            offset += LIMIT;
            fetchDiets();
        });

        btnPrev.setOnClickListener(v -> {
            if (offset >= LIMIT) {
                offset -= LIMIT;
                fetchDiets();
            }
        });

        btnLogout.setOnClickListener(v -> {
        });

        // Handle account details button
        btnAccountDetails.setOnClickListener(v -> {
            fetchAccountDetails();
        });

        // Handle cart button
        btnCart.setOnClickListener(v -> {
            if (CartManager.getInstance().getCart().isEmpty()) {
                // Show a message if the cart is empty
                Toast.makeText(DietsActivity.this, "Your cart is empty. Please add items before proceeding.", Toast.LENGTH_SHORT).show();
            } else {
                startActivity(new Intent(DietsActivity.this, CartActivity.class));
            }
        });

        fetchDiets();
    }

    private void fetchDiets() {
        progressBar.setVisibility(View.VISIBLE);

        String url = "https://10.0.2.2:5001/Diets?Offset=" + offset + "&Limit=" + LIMIT;

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(DietsActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                });
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                runOnUiThread(() -> progressBar.setVisibility(View.GONE));

                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(DietsActivity.this, "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                    return;
                }

                String responseBody = response.body().string();

                try {
                    JSONArray jsonArray = new JSONArray(responseBody);
                    List<Diet> newDiets = new ArrayList<>();

                    for (int i = 0; i < jsonArray.length(); i++) {
                        JSONObject dietObj = jsonArray.getJSONObject(i);
                        Diet diet = new Diet();
                        diet.setId(dietObj.getString("id"));
                        diet.setName(dietObj.getString("name"));
                        diet.setPrice(dietObj.getInt("price"));
                        diet.setCalories(dietObj.getInt("calories"));
                        diet.setVegan(dietObj.getBoolean("vegan"));
                        newDiets.add(diet);
                    }

                    runOnUiThread(() -> {
                        dietList.clear();
                        dietList.addAll(newDiets);

                        btnNext.setEnabled(dietList.size() >= LIMIT);
                        btnPrev.setEnabled(offset > 0);

                        adapter.notifyDataSetChanged();
                    });

                } catch (Exception e) {
                    runOnUiThread(() -> Toast.makeText(DietsActivity.this, "Error parsing response", Toast.LENGTH_SHORT).show());
                }
            }
        });
    }

    private void fetchAccountDetails() {
        String url = "https://10.0.2.2:5001/Client/account";

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                runOnUiThread(() -> {
                    Toast.makeText(DietsActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                });
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    runOnUiThread(() -> Toast.makeText(DietsActivity.this, "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                    return;
                }

                try {
                    String responseBody = response.body().string();
                    JSONObject accountJson = new JSONObject(responseBody);

                    String name = accountJson.getString("name");
                    String lastName = accountJson.getString("lastName");
                    String email = accountJson.getString("email");
                    String phoneNumber = accountJson.getString("phoneNumber");

                    JSONObject address = accountJson.getJSONObject("address");
                    String street = address.getString("street");
                    String buildingNumber = address.getString("buildingNumber");
                    String apartmentNumber = address.getString("apartmentNumber");
                    String postCode = address.getString("postCode");
                    String city = address.getString("city");

                    // Prepare to open the new screen with account details
                    Intent intent = new Intent(DietsActivity.this, MainAccountActivity.class);
                    intent.putExtra("name", name);
                    intent.putExtra("lastName", lastName);
                    intent.putExtra("email", email);
                    intent.putExtra("phoneNumber", phoneNumber);
                    intent.putExtra("street", street);
                    intent.putExtra("buildingNumber", buildingNumber);
                    intent.putExtra("apartmentNumber", apartmentNumber);
                    intent.putExtra("postCode", postCode);
                    intent.putExtra("city", city);
                    startActivity(intent);

                } catch (Exception e) {
                    runOnUiThread(() -> Toast.makeText(DietsActivity.this, "Error parsing account details", Toast.LENGTH_SHORT).show());
                }
            }
        });
    }
}
