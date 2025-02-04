package com.example.vulnerableapp;

import com.example.vulnerableapp.servermodels.Diet;

import java.util.HashMap;
import java.util.Map;

public class CartManager {

    private static CartManager instance;
    private static Map<Diet, Integer> cart;

    // Private constructor to enforce singleton pattern
    private CartManager() {
        cart = new HashMap<>();
    }

    // Get the single instance of CartManager
    public static synchronized CartManager getInstance() {
        if (instance == null) {
            instance = new CartManager();
        }
        return instance;
    }

    // Add item to cart
    public void addToCart(Diet diet) {
        cart.put(diet, 1);
    }

    // Remove item from cart
    public void removeFromCart(Diet diet) {
        cart.remove(diet);
    }

    // Check if item is in cart
    public boolean isInCart(Diet diet) {
        return cart.containsKey(diet);
    }

    public static int getQuantity(Diet diet) {
        return cart.getOrDefault(diet, 0);
    }

    // Get all cart items
    public Map<Diet, Integer> getCart() {
        return cart;
    }

    // Clear the cart (if needed)
    public void clearCart() {
        cart.clear();
    }
}