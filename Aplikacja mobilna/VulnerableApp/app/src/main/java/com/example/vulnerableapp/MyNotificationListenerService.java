package com.example.vulnerableapp;

import android.content.Intent;
import android.service.notification.NotificationListenerService;
import android.service.notification.StatusBarNotification;
import android.util.Log;

public class MyNotificationListenerService extends NotificationListenerService {

    private static final String TAG = "NotificationListener";

    @Override
    public void onNotificationPosted(StatusBarNotification sbn) {
        super.onNotificationPosted(sbn);
        Log.d(TAG, "Notification Posted: " + sbn.getPackageName() + " - " + sbn.getNotification().extras.getString("android.title"));
    }

    @Override
    public void onNotificationRemoved(StatusBarNotification sbn) {
        super.onNotificationRemoved(sbn);
        Log.d(TAG, "Notification Removed: " + sbn.getPackageName());
    }

    /**
     * Pulls the list of all active notifications and logs them.
     */
    public void logActiveNotifications() {
        StatusBarNotification[] activeNotifications = getActiveNotifications();
        if (activeNotifications != null) {
            for (StatusBarNotification sbn : activeNotifications) {
                Log.d(TAG, "Active Notification: " +
                        "Package: " + sbn.getPackageName() +
                        ", Title: " + sbn.getNotification().extras.getString("android.title") +
                        ", Text: " + sbn.getNotification().extras.getString("android.text"));
            }
        } else {
            Log.d(TAG, "No active notifications.");
        }
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        if (intent != null && "com.example.LOG_NOTIFICATIONS".equals(intent.getAction())) {
            logActiveNotifications();
        }
        return START_STICKY;
    }
}