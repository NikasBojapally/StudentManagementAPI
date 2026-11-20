using Microsoft.Data.SqlClient;

namespace StudentManagementAPI.Database
{
    /// <summary>
    /// Creates the StudentAPI database and Students table (with sample data)
    /// on startup if they do not already exist. Mirrors Database/Schema.sql.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(string? connectionString, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            var builder = new SqlConnectionStringBuilder(connectionString);
            var dbName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName)) dbName = "StudentAPI";

            try
            {
                // 1. Create the database (connect to master first)
                var masterBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
                using (var master = new SqlConnection(masterBuilder.ConnectionString))
                {
                    master.Open();
                    var createDb = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @name) CREATE DATABASE [{dbName.Replace("]", "]]")}];";
                    using var cmd = new SqlCommand(createDb, master);
                    cmd.Parameters.AddWithValue("@name", dbName);
                    cmd.ExecuteNonQuery();
                }

                // 2. Create the Students table + sample data
                const string createTable = @"
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Students')
BEGIN
    CREATE TABLE Students (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Course NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME DEFAULT GETUTCDATE()
    );

    CREATE NONCLUSTERED INDEX IX_Students_Email ON Students(Email);

    INSERT INTO Students (Name, Email, Course) VALUES
        ('John Doe', 'john.doe@example.com', 'Computer Science'),
        ('Jane Smith', 'jane.smith@example.com', 'Software Engineering'),
        ('Mike Johnson', 'mike.johnson@example.com', 'Data Science'),
        ('Sarah Williams', 'sarah.williams@example.com', 'Web Development');

    PRINT 'Students table created successfully with sample data.';
END
ELSE
BEGIN
    PRINT 'Students table already exists.';
END";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using var cmd = new SqlCommand(createTable, conn);
                    cmd.ExecuteNonQuery();
                }

                logger.LogInformation("Database '{Db}' and Students table verified.", dbName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed. Check the connection string in appsettings.json.");
                throw;
            }
        }
    }
}
