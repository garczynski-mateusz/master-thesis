package com.example.vulnerableapp;

import android.content.Context;
import android.content.SharedPreferences;

public class TokenManager {

    private static final String SHARED_PREFS_NAME = "app_prefs";
    private static final String TOKEN_KEY = "jwt_token";

    // Save the token in SharedPreferences
    public static void saveToken(Context context, String token) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(SHARED_PREFS_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(TOKEN_KEY, token);
        editor.apply(); // Save the token asynchronously
    }

    // Retrieve the token from SharedPreferences
    public static String getToken(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(SHARED_PREFS_NAME, Context.MODE_PRIVATE);
        return sharedPreferences.getString(TOKEN_KEY, null); // Return null if token is not found
    }

    // Clear the token (e.g., on logout)
    public static void clearToken(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(SHARED_PREFS_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove(TOKEN_KEY);
        editor.apply(); // Clear the token asynchronously
    }
}