package com.example.vulnerableapp;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.vulnerableapp.adapters.OrderAdapter;
import com.example.vulnerableapp.servermodels.Order;

import java.util.List;

public class OrderHistoryFragment extends Fragment {
    private List<Order> orders;
    OrderAdapter adapter;

    public void updateOrderList(List<Order> orders) {
        this.orders = orders;
        if (adapter != null) adapter.updateOrders(orders);
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_order_history, container, false);

        RecyclerView recyclerView = view.findViewById(R.id.recycler_view);
        recyclerView.setLayoutManager(new LinearLayoutManager(getContext()));

        if (getArguments() != null) orders = (List<Order>) getArguments().getSerializable("orders");

        adapter = new OrderAdapter(orders);
        recyclerView.setAdapter(adapter);

        return view;
    }
}
