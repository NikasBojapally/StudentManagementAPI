-- Create StudentAPI Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StudentAPI')
BEGIN
    CREATE DATABASE StudentAPI;
END
GO

-- Use the StudentAPI database
USE StudentAPI;
GO

-- Create Students Table
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

    -- Create index on Email for faster lookups
    CREATE NONCLUSTERED INDEX IX_Students_Email ON Students(Email);

    -- Insert sample data
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
END
GO
