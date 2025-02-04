package com.example.vulnerableapp;

import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.provider.Settings;

import androidx.core.app.NotificationCompat;

import java.util.Calendar;


public class ImplicitNotificationHelper {
    // Method to send a one-off notification
    public static void sendNotification(Context context, String title, String message) {
        String channelId = "scheduled_notification_channel";
        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(
                    channelId,
                    "Scheduled Notifications",
                    NotificationManager.IMPORTANCE_HIGH
            );
            channel.setLockscreenVisibility(Notification.VISIBILITY_PUBLIC);
            notificationManager.createNotificationChannel(channel);
        }

        NotificationCompat.Builder builder = new NotificationCompat.Builder(context, channelId)
                .setSmallIcon(android.R.drawable.ic_dialog_info)
                .setContentTitle(title)
                .setContentText(message)
                .setPriority(NotificationCompat.PRIORITY_HIGH)
                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

        notificationManager.notify((int) System.currentTimeMillis(), builder.build());

        // Show currently existing notifications
        Intent intent = new Intent("com.example.LOG_NOTIFICATIONS");
        intent.setClass(context, MyNotificationListenerService.class);
        context.startService(intent);
    }

    // Method to schedule a notification at a specific time
    public static void scheduleNotification(Context context, String title, String message) {
        AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
        if (alarmManager != null && !alarmManager.canScheduleExactAlarms()) {
            // Permission is not granted
            Intent intent = new Intent(Settings.ACTION_REQUEST_SCHEDULE_EXACT_ALARM);
            context.startActivity(intent);
        }
        IntentFilter filter = new IntentFilter("android.intent.action.MAIN");
        context.registerReceiver(new ImplicitNotificationReceiver(), filter, Context.RECEIVER_EXPORTED);

        Intent intent = new Intent("android.intent.action.MAIN");
        intent.putExtra("title", title);
        intent.putExtra("message", message);

        PendingIntent pendingIntent = PendingIntent.getBroadcast(context, 0, intent,
                PendingIntent.FLAG_IMMUTABLE | PendingIntent.FLAG_UPDATE_CURRENT);

        // Set the alarm time to 10 seconds from now
        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.SECOND, 10); // Add 10 seconds to the current time

        // Schedule the alarm
        if (alarmManager != null) {
            alarmManager.setExact(AlarmManager.RTC_WAKEUP, calendar.getTimeInMillis(), pendingIntent);
        }
    }
}
