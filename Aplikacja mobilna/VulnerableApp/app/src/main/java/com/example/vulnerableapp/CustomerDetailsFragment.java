package com.example.vulnerableapp;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.google.gson.Gson;

import org.json.JSONObject;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class CustomerDetailsFragment extends Fragment {

    private EditText etName, etLastName, etEmail, etPhoneNumber, etStreet, etBuildingNumber,
            etApartmentNumber, etPostCode, etCity, etPassword, etConfirmPassword;
    private Button btnSave;

    private static final String ARG_NAME = "name";
    private static final String ARG_LAST_NAME = "lastName";
    private static final String ARG_EMAIL = "email";
    private static final String ARG_PHONE_NUMBER = "phoneNumber";
    private static final String ARG_STREET = "street";
    private static final String ARG_BUILDING_NUMBER = "buildingNumber";
    private static final String ARG_APARTMENT_NUMBER = "apartmentNumber";
    private static final String ARG_POST_CODE = "postCode";
    private static final String ARG_CITY = "city";

    public static CustomerDetailsFragment newInstance(String name, String lastName, String email, String phoneNumber,
                                                      String street, String buildingNumber, String apartmentNumber,
                                                      String postCode, String city) {
        CustomerDetailsFragment fragment = new CustomerDetailsFragment();
        Bundle args = new Bundle();
        args.putString(ARG_NAME, name);
        args.putString(ARG_LAST_NAME, lastName);
        args.putString(ARG_EMAIL, email);
        args.putString(ARG_PHONE_NUMBER, phoneNumber);
        args.putString(ARG_STREET, street);
        args.putString(ARG_BUILDING_NUMBER, buildingNumber);
        args.putString(ARG_APARTMENT_NUMBER, apartmentNumber);
        args.putString(ARG_POST_CODE, postCode);
        args.putString(ARG_CITY, city);
        fragment.setArguments(args);
        return fragment;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_customer_details, container, false);


        etName = view.findViewById(R.id.etName);
        etLastName = view.findViewById(R.id.etLastName);
        etEmail = view.findViewById(R.id.etEmail);
        etPhoneNumber = view.findViewById(R.id.etPhoneNumber);
        etStreet = view.findViewById(R.id.etStreet);
        etBuildingNumber = view.findViewById(R.id.etBuildingNumber);
        etApartmentNumber = view.findViewById(R.id.etApartmentNumber);
        etPostCode = view.findViewById(R.id.etPostCode);
        etCity = view.findViewById(R.id.etCity);
        etPassword = view.findViewById(R.id.etPassword);
        etConfirmPassword = view.findViewById(R.id.etConfirmPassword);
        btnSave = view.findViewById(R.id.btnSave);
        btnSave.setEnabled(false);

        if (getArguments() != null) {
            etName.setText(getArguments().getString(ARG_NAME, ""));
            etLastName.setText(getArguments().getString(ARG_LAST_NAME, ""));
            etEmail.setText(getArguments().getString(ARG_EMAIL, ""));
            etPhoneNumber.setText(getArguments().getString(ARG_PHONE_NUMBER, ""));
            etStreet.setText(getArguments().getString(ARG_STREET, ""));
            etBuildingNumber.setText(getArguments().getString(ARG_BUILDING_NUMBER, ""));
            etApartmentNumber.setText(getArguments().getString(ARG_APARTMENT_NUMBER, ""));
            etPostCode.setText(getArguments().getString(ARG_POST_CODE, ""));
            etCity.setText(getArguments().getString(ARG_CITY, ""));
        }

        TextWatcher passwordTextWatcher = new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
                validatePasswords();
            }

            @Override
            public void afterTextChanged(Editable editable) {
            }

            private void validatePasswords() {
                String password = etPassword.getText().toString();
                String confirmPassword = etConfirmPassword.getText().toString();
                // Enable button only if both fields are non-empty and match
                btnSave.setEnabled(!password.isEmpty() && !confirmPassword.isEmpty() && password.equals(confirmPassword));
            }
        };

        etPassword.addTextChangedListener(passwordTextWatcher);
        etConfirmPassword.addTextChangedListener(passwordTextWatcher);

        btnSave.setOnClickListener(v -> {
            // Prepare JSON payload
            JSONObject payload = new JSONObject();
            try {
                payload.put("name", etName.getText().toString());
                payload.put("lastName", etLastName.getText().toString());
                payload.put("email", etEmail.getText().toString());
                payload.put("password", etPassword.getText().toString());
                payload.put("phoneNumber", etPhoneNumber.getText().toString());

                JSONObject address = new JSONObject();
                address.put("street", etStreet.getText().toString());
                address.put("buildingNumber", etBuildingNumber.getText().toString());
                address.put("apartmentNumber", etApartmentNumber.getText().toString());
                address.put("postCode", etPostCode.getText().toString());
                address.put("city", etCity.getText().toString());

                payload.put("address", address);
            } catch (Exception e) {
                e.printStackTrace();
            }

            // Perform PUT request
            String url = "https://10.0.2.2:5001/Client/account";
            String token = TokenManager.getToken(requireContext());

            Request request = new Request.Builder()
                    .url(url)
                    .header("Authorization", "Bearer " + token)
                    .put(RequestBody.create(payload.toString(), MediaType.parse("application/json")))
                    .build();

            // Use OkHttpClient for async call
            OkHttpClient client = UnsafeOkHttpClient.getUnsafeOkHttpClient(); // Use your unsafe OkHttp client setup
            client.newCall(request).enqueue(new Callback() {
                @Override
                public void onFailure(Call call, IOException e) {
                    requireActivity().runOnUiThread(() -> Toast.makeText(requireContext(),"Error: " + e.getMessage(), Toast.LENGTH_SHORT).show());
                }

                @Override
                public void onResponse(Call call, Response response) throws IOException {
                    if (!response.isSuccessful()) {
                        requireActivity().runOnUiThread(() -> Toast.makeText(requireContext(), "Error: " + response.message(), Toast.LENGTH_SHORT).show());
                        return;
                    }

                    requireActivity().runOnUiThread(() -> {
                        Toast.makeText(requireContext(), "Account details saved successfully", Toast.LENGTH_SHORT).show();
                        Gson gson = new Gson();
                        String json = gson.toJson(payload);
                        Log.d("ObjectLogger", "Successfully changed account details" + json);
                    });
                }
            });
        });

        return view;
    }
}
