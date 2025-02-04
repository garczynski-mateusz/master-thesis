package com.example.vulnerableapp.adapters;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.R;
import com.example.vulnerableapp.servermodels.Order;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class OrderDietAdapter extends RecyclerView.Adapter<OrderDietAdapter.DietViewHolder> {

    private List<Order.Diet> diets;
    private Set<Integer> expandedPositions = new HashSet<>();

    public OrderDietAdapter(List<Order.Diet> diets) {
        this.diets = diets;
    }

    @NonNull
    @Override
    public DietViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_order_diet, parent, false);
        return new DietViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull DietViewHolder holder, int position) {
        Order.Diet diet = diets.get(position);
        holder.bind(diet, position);
    }

    @Override
    public int getItemCount() {
        return diets.size();
    }

    public class DietViewHolder extends RecyclerView.ViewHolder {
        private TextView dietNameTextView, dietPriceTextView;
        private ImageView imgVegan;
        private RecyclerView mealRecyclerView;

        public DietViewHolder(@NonNull View itemView) {
            super(itemView);
            dietNameTextView = itemView.findViewById(R.id.diet_name);
            dietPriceTextView = itemView.findViewById(R.id.diet_price);
            imgVegan = itemView.findViewById(R.id.imgVegan);
            mealRecyclerView = itemView.findViewById(R.id.meal_recycler_view);
            mealRecyclerView.setNestedScrollingEnabled(false);
        }

        public void bind(Order.Diet diet, int position) {
            dietNameTextView.setText(diet.name);
            dietPriceTextView.setText("$" + diet.price);
            imgVegan.setVisibility(diet.vegan ? View.VISIBLE : View.GONE);

            // Set up meal RecyclerView
            mealRecyclerView.setLayoutManager(new LinearLayoutManager(itemView.getContext()));
            OrderMealAdapter mealAdapter = new OrderMealAdapter(itemView.getContext(), diet.meals);
            mealRecyclerView.setAdapter(mealAdapter);

            // Toggle visibility of mealRecyclerView
            mealRecyclerView.setVisibility(expandedPositions.contains(position) ? View.VISIBLE : View.GONE);

            // Handle click to expand/collapse
            itemView.setOnClickListener(v -> {
                if (expandedPositions.contains(position)) {
                    expandedPositions.remove(position); // Collapse
                } else {
                    expandedPositions.add(position); // Expand
                }
                notifyItemChanged(position); // Refresh the item
            });
        }
    }
}