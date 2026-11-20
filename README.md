# StudentManagementAPI

ASP.NET Core (.NET 8) REST API for managing students, backed by SQL Server.

## Run
1. Install the .NET 8 SDK and SQL Server (or LocalDB).
2. Edit `ConnectionStrings:DefaultConnection` in `appsettings.json` if you are not using LocalDB.
3. `dotnet restore`
4. `dotnet run`
5. Open http://localhost:5000/swagger

The database `StudentAPI` and the `Students` table (with sample data) are created automatically on startup.
`Database/Schema.sql` is the same script if you prefer to run it manually in SSMS.

## Endpoints
| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/students | List all students |
| GET | /api/students/{id} | Get one student |
| POST | /api/students | Add a student |
| PUT | /api/students/{id} | Update a student |
| DELETE | /api/students/{id} | Delete a student |

Test requests are in `StudentManagementAPI.http`.
