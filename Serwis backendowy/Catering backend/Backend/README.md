# Backend

## How to run? (development version)

Backend projects require .Net installed. Project is developed and tested on Windows 10/11 operating systems. It is highly suggested to use Visual Studio IDE to run and develop the project. There is also a possibility to build/test/run this project from CLI which is explained below.

### 1. Building
To build the project it is needed to execute the following command inside of the `./Backend` directory:
```
dotnet build
```

### 2. Testing
To test the project you need to execute the following command inside of the `./Backend` directory:
```
dotnet test
```

### 3. Running
To run the backend application you need to execute the following command inside of the `./Backend/CateringBackend` directory:
```
dotnet run
```

### 4. Viewing swagger API page
After running the backend application, the swagger API page can be accessed at the following URL:
```
https://localhost:5001/swagger/index.html
```

## Migrations

### How the migration process works?

Any pending migrations are applied inside the constructor of the `CateringDbContext` object. This way any new migrations are applied instantly without the need of restarting the application.

### How to add migration

To add the migration you need to execute the following command inside of the `./Backend/CateringBackend` directory:
```
dotnet ef migrations add <migration-name> --project ..\CateringBackend.Domain\CateringBackend.Domain.csproj
```

### How to unapply a migration? 

To unapply the migration you need to execute the following command inside of the `./Backend/CateringBackend` directory:
```
dotnet ef database update <previous-migration-name> --project ..\CateringBackend.Domain\CateringBackend.Domain.csproj
```

### The current state of migrations

The whole database structure and seeding was applied by creating a single migration that is called `20220318152158_Initial`, no other migrations were created.