package com.example.vulnerableapp.adapters;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.R;
import com.example.vulnerableapp.servermodels.Diet;

import java.util.Map;

public class CartAdapter extends RecyclerView.Adapter<CartAdapter.CartViewHolder> {

    private final Map<Diet, Integer> cartItems;

    public CartAdapter(Map<Diet, Integer> cartItems) {
        this.cartItems = cartItems;
    }

    @NonNull
    @Override
    public CartViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_cart, parent, false);
        return new CartViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull CartViewHolder holder, int position) {
        Diet diet = (Diet) cartItems.keySet().toArray()[position];
        int totalPrice = diet.getPrice();

        holder.tvDietName.setText(diet.getName());
        holder.tvDietTotalPrice.setText("Total Price: " + totalPrice);
    }

    @Override
    public int getItemCount() {
        return cartItems.size();
    }

    static class CartViewHolder extends RecyclerView.ViewHolder {
        TextView tvDietName,  tvDietTotalPrice;

        public CartViewHolder(@NonNull View itemView) {
            super(itemView);
            tvDietName = itemView.findViewById(R.id.tvDietName);
            tvDietTotalPrice = itemView.findViewById(R.id.tvDietTotalPrice);
        }
    }
}
