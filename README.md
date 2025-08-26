# AllTheBeans (EF Core)

Demo .NET 8 minimal API using EF Core (SQL Server) and a simple static front-end.

## Quick start

1. Option A: Use LocalDB (Windows)
   - Ensure `(localdb)\\MSSQLLocalDB` is available.
   - Run: `dotnet run`
   - The app will `EnsureCreated()` the database and seed from `Data/AllTheBeans.json` on first run.

2. Option B: Create DB manually
   - Run `sql/init.sql` in SQL Server.
   - Update `appsettings.json` connection string if needed.
   - Run the app.

Open: http://localhost:5000
