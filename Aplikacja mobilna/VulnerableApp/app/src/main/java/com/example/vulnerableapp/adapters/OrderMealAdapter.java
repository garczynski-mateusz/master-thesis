package com.example.vulnerableapp.adapters;

import android.content.Context;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.R;
import com.example.vulnerableapp.servermodels.Order;

import java.util.List;

public class OrderMealAdapter extends RecyclerView.Adapter<OrderMealAdapter.MealViewHolder> {

    private List<Order.Meal> meals;
    private Context context;

    public OrderMealAdapter(Context context, List<Order.Meal> meals) {
        this.context = context;
        this.meals = meals;
    }

    @NonNull
    @Override
    public MealViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_order_meal, parent, false);
        return new MealViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull MealViewHolder holder, int position) {
        Order.Meal meal = meals.get(position);
        holder.bind(meal);
    }

    @Override
    public int getItemCount() {
        return meals.size();
    }

    public class MealViewHolder extends RecyclerView.ViewHolder {
        private TextView mealNameTextView;
        private TextView mealCaloriesTextView;
        private ImageView magnifyingGlassIcon;

        public MealViewHolder(@NonNull View itemView) {
            super(itemView);
            mealNameTextView = itemView.findViewById(R.id.meal_name);
            mealCaloriesTextView = itemView.findViewById(R.id.meal_calories);
            magnifyingGlassIcon = itemView.findViewById(R.id.magnifying_glass_icon);
        }

        public void bind(Order.Meal meal) {
            mealNameTextView.setText(meal.name);
            mealCaloriesTextView.setText(meal.calories + " kcal");

            magnifyingGlassIcon.setOnClickListener(v -> {
                // Call the popup method with meal details
                showMealDetailsPopup(
                        meal.name,
                        meal.vegan,
                        meal.ingredientList,
                        meal.allergenList,
                        meal.calories
                );
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
    }
}
