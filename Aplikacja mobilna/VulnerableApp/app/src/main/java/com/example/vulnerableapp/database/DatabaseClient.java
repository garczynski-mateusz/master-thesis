package com.example.vulnerableapp.database;

import android.content.Context;
import androidx.room.Room;

import net.sqlcipher.database.SQLiteDatabase;
import net.sqlcipher.database.SupportFactory;

public class DatabaseClient {
    private static AppDatabase appDatabase;

    public static AppDatabase getDatabase(Context context) {
        if (appDatabase == null) {
            SQLiteDatabase.loadLibs(context);
            SupportFactory factory = new SupportFactory("password".getBytes());
            appDatabase = Room.databaseBuilder(context.getApplicationContext(),
                    AppDatabase.class, "order_database")
                    .fallbackToDestructiveMigration()
                    .openHelperFactory(factory)
                    .build();
        }
        return appDatabase;
    }
}