package com.example.vulnerableapp.adapters;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.R;
import com.example.vulnerableapp.servermodels.Order;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

import java.util.List;

public class OrderAdapter extends RecyclerView.Adapter<OrderAdapter.OrderViewHolder> {

    private List<Order> orders;

    public OrderAdapter(List<Order> orders) {
        this.orders = orders;
    }

    public void updateOrders(List<Order> orders) {
        this.orders = orders;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public OrderViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_order, parent, false);
        return new OrderViewHolder(view);
    }

    @Override
    public void onBindViewHolder(OrderViewHolder holder, int position) {
        Order order = orders.get(position);
        holder.bind(order);
    }

    @Override
    public int getItemCount() {
        if (orders == null) return 0;
        return orders.size();
    }

    public static class OrderViewHolder extends RecyclerView.ViewHolder {
        private TextView orderIdTextView, orderStatusTextView, orderDatesTextView, orderPriceTextView, orderDeliveryDetailsTextView;
        private RecyclerView dietRecyclerView;
        private boolean isExpanded = false;

        public OrderViewHolder(@NonNull View itemView) {
            super(itemView);
            orderIdTextView = itemView.findViewById(R.id.order_id);
            orderStatusTextView = itemView.findViewById(R.id.order_status);
            orderDatesTextView = itemView.findViewById(R.id.order_dates);
            orderPriceTextView = itemView.findViewById(R.id.order_price);
            orderDeliveryDetailsTextView = itemView.findViewById(R.id.order_delivery_details);
            dietRecyclerView = itemView.findViewById(R.id.diet_recycler_view);
        }

        public void bind(Order order) {
            String formattedStartDate = formatDate(order.startDate);
            String formattedEndDate = formatDate(order.endDate);

            orderIdTextView.setText(order.id);
            orderStatusTextView.setText(order.status);
            orderDatesTextView.setText("From: " + formattedStartDate + " To: " + formattedEndDate);
            orderPriceTextView.setText("$" + order.price);
            orderDeliveryDetailsTextView.setText(order.deliveryDetails.address.street + " " +
                    order.deliveryDetails.address.buildingNumber + "/" +
                    order.deliveryDetails.address.apartmentNumber + ", " +
                    order.deliveryDetails.address.postCode + " " +
                    order.deliveryDetails.address.city);

            // Set up nested RecyclerView for diets
            OrderDietAdapter dietAdapter = new OrderDietAdapter(order.diets);
            dietRecyclerView.setLayoutManager(new LinearLayoutManager(itemView.getContext()));
            dietRecyclerView.setAdapter(dietAdapter);

            // Toggle expansion
            itemView.setOnClickListener(v -> {
                isExpanded = !isExpanded;
                dietRecyclerView.setVisibility(isExpanded ? View.VISIBLE : View.GONE);
            });
        }

        private String formatDate(String dateTime) {
            try {
                // Parse the date-time string
                LocalDateTime date = LocalDateTime.parse(dateTime);
                // Format it to yyyy-MM-dd
                DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd");
                return date.format(formatter);
            } catch (Exception e) {
                // Handle parsing errors or return the original date if parsing fails
                return dateTime;
            }
        }
    }
}
