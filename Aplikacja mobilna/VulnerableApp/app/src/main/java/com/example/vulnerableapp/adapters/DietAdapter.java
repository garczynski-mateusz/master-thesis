package com.example.vulnerableapp.adapters;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.CartManager;
import com.example.vulnerableapp.R;
import com.example.vulnerableapp.UnsafeOkHttpClient;
import com.example.vulnerableapp.servermodels.Diet;
import com.example.vulnerableapp.servermodels.Meal;

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

public class DietAdapter extends RecyclerView.Adapter<DietAdapter.DietViewHolder> {

    private List<Diet> dietList;
    private Context context;
    private OkHttpClient client;
    private String token;

    public DietAdapter(List<Diet> dietList, Context context, String token) {
        this.dietList = dietList;
        this.context = context;
        this.client = UnsafeOkHttpClient.getUnsafeOkHttpClient();
        this.token = token; // Pass the token for Authorization
    }

    @NonNull
    @Override
    public DietViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_diet, parent, false);
        return new DietViewHolder(view);
    }

    @SuppressLint("ResourceAsColor")
    @Override
    public void onBindViewHolder(@NonNull DietViewHolder holder, int position) {
        Diet diet = dietList.get(position);
        holder.tvName.setText(diet.getName());
        holder.tvPrice.setText("Price: " + diet.getPrice());
        holder.tvCalories.setText("Calories: " + diet.getCalories());

        // Show the vegan leaf icon if the diet is vegan, otherwise hide it
        if (diet.isVegan()) {
            holder.imgVegan.setVisibility(View.VISIBLE);  // Show the leaf icon
        } else {
            holder.imgVegan.setVisibility(View.GONE);  // Hide the leaf icon
        }

        if (CartManager.getInstance().isInCart(diet)) {
            // If in cart, show trash icon
            holder.btnAdd.setImageResource(R.drawable.ic_trash);
        } else {
            // If not in cart, show add "+" icon
            holder.btnAdd.setImageResource(android.R.drawable.ic_input_add);
        }

        // Handle click event for the + button
        holder.btnAdd.setOnClickListener(v -> {
            if (CartManager.getInstance().isInCart(diet)) {
                // Remove from cart if it's already there and switch to "+" icon
                CartManager.getInstance().removeFromCart(diet);
                holder.btnAdd.setImageResource(android.R.drawable.ic_input_add);
                Toast.makeText(context, "Diet removed from cart", Toast.LENGTH_SHORT).show();
            } else {
                // Add to cart with quantity of 1 and switch to trash icon
                CartManager.getInstance().addToCart(diet);
                holder.btnAdd.setImageResource(R.drawable.ic_trash);
                Toast.makeText(context, "Diet added to cart", Toast.LENGTH_SHORT).show();
            }
        });

        // Set visibility and content based on the diet's expanded state
        if (diet.isExpanded()) {
            holder.mealContainer.setVisibility(View.VISIBLE);
            holder.mealContainer.removeAllViews(); // Clear previous views before adding new ones

            if (diet.getMeals() == null || diet.getMeals().isEmpty()) {
                // Show loading message if meals list is not yet available
                TextView loadingText = new TextView(context);
                loadingText.setText("Loading...");
                holder.mealContainer.addView(loadingText);
            } else {
                // Dynamically add each meal with a magnifier button
                for (Meal meal : diet.getMeals()) {
                    LinearLayout mealLayout = new LinearLayout(context);
                    mealLayout.setOrientation(LinearLayout.HORIZONTAL);

                    // Create TextView for the meal
                    TextView mealText = new TextView(context);
                    mealText.setText(meal.getName() + " - " + meal.getCalories() + " kcal");

                    // Add layout parameters to TextView (optional - adjust as needed)
                    LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(
                            0, // Set width to 0 so it can be weight-based
                            LinearLayout.LayoutParams.WRAP_CONTENT,
                            1f // Weight to distribute available space
                    );
                    mealText.setLayoutParams(params); // Apply the layout parameters to the TextView

                    // Create ImageButton with a magnifier icon
                    ImageButton magnifierButton = new ImageButton(context);
                    magnifierButton.setImageResource(android.R.drawable.ic_menu_search); // Using default Android magnifier icon
                    magnifierButton.setBackgroundColor(android.R.color.transparent); // Transparent background for the button
                    magnifierButton.setOnClickListener(v -> {
                        // Handle magnifier button click
                        fetchMealDetails(meal.getId(), holder);
                    });

                    // Add the meal TextView and magnifier ImageButton to the horizontal layout
                    mealLayout.addView(mealText);
                    mealLayout.addView(magnifierButton);

                    // Add the meal layout to the meal container
                    holder.mealContainer.addView(mealLayout);
                }
            }
        } else {
            holder.mealContainer.setVisibility(View.GONE); // Hide the meals when collapsed
        }

        // Handle click to load detailed diet data and meals
        holder.itemView.setOnClickListener(v -> {
            diet.setExpanded(!diet.isExpanded());
            notifyItemChanged(position); // Notify about change in expanded state

            if (diet.isExpanded()) { // If expanding
                if (diet.getMeals() == null || diet.getMeals().isEmpty()) { // Check if meals list is not yet fetched
                    fetchDietDetails(diet, holder, position); // Pass the position
                }
            }
        });
    }

    private void fetchMealDetails(String mealId, DietViewHolder holder) {
        String url = "https://10.0.2.2:5001/Meals/" + mealId;

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                (holder).itemView.post(() -> {
                    Toast.makeText(context, "Failed to fetch meal details", Toast.LENGTH_SHORT).show();
                });
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    (holder).itemView.post(() -> {
                        Toast.makeText(context, "Error: " + response.message(), Toast.LENGTH_SHORT).show();
                    });
                    return;
                }

                String responseBody = response.body().string();
                try {
                    JSONObject jsonObject = new JSONObject(responseBody);
                    String mealName = jsonObject.getString("name");
                    int calories = jsonObject.getInt("calories");
                    boolean isVegan = jsonObject.getBoolean("vegan");

                    JSONArray ingredientsArray = jsonObject.getJSONArray("ingredientList");
                    JSONArray allergensArray = jsonObject.getJSONArray("allergenList");

                    List<String> ingredients = new ArrayList<>();
                    for (int i = 0; i < ingredientsArray.length(); i++) {
                        ingredients.add(ingredientsArray.getString(i));
                    }

                    List<String> allergens = new ArrayList<>();
                    for (int i = 0; i < allergensArray.length(); i++) {
                        allergens.add(allergensArray.getString(i));
                    }

                    ((DietViewHolder) holder).itemView.post(() -> {
                        showMealDetailsPopup(mealName, isVegan, ingredients, allergens, calories);
                    });

                } catch (Exception e) {
                    e.printStackTrace();
                    ((DietViewHolder) holder).itemView.post(() -> {
                        Toast.makeText(context, "Error parsing meal details", Toast.LENGTH_SHORT).show();
                    });
                }
            }
        });
    }

    private void showMealDetailsPopup(String mealName, boolean isVegan, List<String> ingredients, List<String> allergens, int calories) {
        // Inflate the custom layout for the popup background
        View popupBackgroundView = LayoutInflater.from(context).inflate(R.layout.popup_background, null);

        // Initialize the popup window
        PopupWindow popupWindow = new PopupWindow(popupBackgroundView, ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT, true);

        // Inflate the meal details layout and find the popup container
        View mealDetailsView = LayoutInflater.from(context).inflate(R.layout.meal_details_popup, null);
        LinearLayout popupContainer = popupBackgroundView.findViewById(R.id.popupContainer);
        popupContainer.addView(mealDetailsView);

        // Initialize the UI elements in the meal details view
        TextView mealNameTextView = mealDetailsView.findViewById(R.id.mealName);
        ImageView veganIcon = mealDetailsView.findViewById(R.id.imgVegan);
        LinearLayout ingredientListLayout = mealDetailsView.findViewById(R.id.ingredientList);
        LinearLayout allergenListLayout = mealDetailsView.findViewById(R.id.allergenList);
        TextView mealCaloriesTextView = mealDetailsView.findViewById(R.id.mealCalories);

        // Set the meal name
        mealNameTextView.setText(mealName);

        // Show the vegan icon if the meal is vegan
        if (isVegan) {
            veganIcon.setVisibility(View.VISIBLE);
        } else {
            veganIcon.setVisibility(View.GONE);
        }

        // Add ingredients to the ingredient list layout
        for (String ingredient : ingredients) {
            TextView ingredientTextView = new TextView(context);
            ingredientTextView.setText(ingredient);
            ingredientTextView.setLayoutParams(new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT, // Match width
                    ViewGroup.LayoutParams.WRAP_CONTENT // Wrap content height
            ));
            ingredientListLayout.addView(ingredientTextView);
        }

        // Add allergens to the allergen list layout
        for (String allergen : allergens) {
            TextView allergenTextView = new TextView(context);
            allergenTextView.setText(allergen);
            allergenTextView.setLayoutParams(new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT, // Match width
                    ViewGroup.LayoutParams.WRAP_CONTENT // Wrap content height
            ));
            allergenListLayout.addView(allergenTextView);
        }

        // Set the calories
        mealCaloriesTextView.setText("Calories: " + calories);

        // Show the popup window
        popupWindow.setFocusable(true);
        popupWindow.setOutsideTouchable(true);
        popupWindow.showAtLocation(popupBackgroundView, Gravity.CENTER, 0, 0);

        // Dismiss the popup when the background is clicked
        popupBackgroundView.setOnClickListener(v -> {
            popupWindow.dismiss();
        });
    }

    @Override
    public int getItemCount() {
        return dietList.size();
    }

    public static class DietViewHolder extends RecyclerView.ViewHolder {
        public ViewGroup mealContainer;
        TextView tvName, tvPrice, tvCalories;
        ImageView imgVegan;
        ImageButton btnAdd;

        public DietViewHolder(@NonNull View itemView) {
            super(itemView);
            tvName = itemView.findViewById(R.id.tvName);
            tvPrice = itemView.findViewById(R.id.tvPrice);
            tvCalories = itemView.findViewById(R.id.tvCalories);
            mealContainer = itemView.findViewById(R.id.mealContainer); // LinearLayout for holding meals
            imgVegan = itemView.findViewById(R.id.imgVegan); // ImageView for the vegan icon
            btnAdd = itemView.findViewById(R.id.btnAdd); // + button
        }
    }

    // Method to fetch diet details from /Diets/{dietId}
    private void fetchDietDetails(Diet diet, DietViewHolder holder, int position) {
        String url = "https://10.0.2.2:5001/Diets/" + diet.getId();

        Request request = new Request.Builder()
                .url(url)
                .header("Authorization", "Bearer " + token)
                .build();

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                // Handle failure
                ((DietViewHolder) holder).itemView.post(() -> {
                    Toast.makeText(context, "Failed to fetch diet details", Toast.LENGTH_SHORT).show();
                });
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    ((DietViewHolder) holder).itemView.post(() -> {
                        Toast.makeText(context, "Error: " + response.message(), Toast.LENGTH_SHORT).show();
                    });
                    return;
                }

                // Parse the response body
                String responseBody = response.body().string();
                try {
                    JSONObject jsonObject = new JSONObject(responseBody);

                    String name = jsonObject.getString("name");
                    int price = jsonObject.getInt("price");
                    int calories = jsonObject.getInt("calories");
                    boolean isVegan = jsonObject.getBoolean("vegan");

                    // Parse meals array
                    JSONArray mealsArray = jsonObject.getJSONArray("meals");
                    List<Meal> meals = new ArrayList<>();
                    for (int i = 0; i < mealsArray.length(); i++) {
                        JSONObject mealObj = mealsArray.getJSONObject(i);
                        Meal meal = new Meal();
                        meal.setId(mealObj.getString("id"));
                        meal.setName(mealObj.getString("name"));
                        meal.setCalories(mealObj.getInt("calories"));
                        meal.setVegan(mealObj.getBoolean("isVegan"));
                        meals.add(meal);
                    }

                    diet.setMeals(meals); // Set the fetched meals list in the Diet object

                    // Update the UI with the fetched data (on the main thread)
                    ((DietViewHolder) holder).itemView.post(() -> {
                        notifyItemChanged(position); // Trigger re-bind of the item to update the view
                    });

                } catch (Exception e) {
                    e.printStackTrace();
                    ((DietViewHolder) holder).itemView.post(() -> {
                        Toast.makeText(context, "Error parsing diet details", Toast.LENGTH_SHORT).show();
                    });
                }
            }
        });
    }


}
