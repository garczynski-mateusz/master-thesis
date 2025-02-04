package com.example.vulnerableapp;

import android.content.ComponentName;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.provider.Settings;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import androidx.appcompat.app.AppCompatActivity;


public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        PackageManager pm = getPackageManager();
        ComponentName componentName = new ComponentName(this, NotificationReceiver.class);
        pm.setComponentEnabledSetting(
                componentName,
                PackageManager.COMPONENT_ENABLED_STATE_ENABLED,
                PackageManager.DONT_KILL_APP
        );
        int receiverStatus = pm.getComponentEnabledSetting(componentName);
        Log.d("ReceiverStatus", "Status: " + receiverStatus);

        Button customerLoginButton = findViewById(R.id.CustomerLoginButton);
        Button localOrderHistoryButton = findViewById(R.id.LocalOrderHistoryButton);

        customerLoginButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Create an Intent to start LoginActivity
                Intent intent = new Intent(MainActivity.this, CustomerLoginActivity.class);
                startActivity(intent); // Start the LoginActivity
            }
        });

        localOrderHistoryButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Create an Intent to start LoginActivity
                Intent intent = new Intent(MainActivity.this, LocalOrderHistoryActivity.class);
                startActivity(intent); // Start the LocalOrderHistoryActivity
            }
        });
    }
}