package com.example.vulnerableapp;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class NotificationReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        // Trigger the notification when the alarm fires
        String action = intent.getAction();

        if ("android.intent.action.MAIN".equals(action)) {
            String title = intent.getStringExtra("title");
            String message = intent.getStringExtra("message");

            NotificationHelper.sendNotification(context, title, message);
        }
    }
}
