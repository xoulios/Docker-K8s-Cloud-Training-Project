IF DB_ID(N'MovieStreamingDb') IS NULL
BEGIN
    CREATE DATABASE [MovieStreamingDb];
END;
GO

USE [MovieStreamingDb];
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'movieapi')
BEGIN
    CREATE LOGIN [movieapi] WITH PASSWORD = N'Movie_Api_2026!', CHECK_POLICY = OFF;
END;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'movieapi')
BEGIN
    CREATE USER [movieapi] FOR LOGIN [movieapi];
    ALTER ROLE [db_owner] ADD MEMBER [movieapi];
END;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260905203438_InitialCreate'
)
BEGIN
    CREATE TABLE [Movies] (
        [Code] nvarchar(10) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Director] nvarchar(200) NOT NULL,
        [Year] int NOT NULL,
        [DurationMinutes] int NOT NULL,
        [Language] nvarchar(50) NOT NULL,
        [Rating] decimal(3,1) NOT NULL,
        [Seasons] int NOT NULL,
        [RentalCost] decimal(8,2) NOT NULL,
        [Genres] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Movies] PRIMARY KEY ([Code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260905203438_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Movies_Title_Year_Director] ON [Movies] ([Title], [Year], [Director]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260905203438_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260905203438_InitialCreate', N'10.0.0');
END;

COMMIT;
GO

IF NOT EXISTS (SELECT 1 FROM [Movies])
BEGIN
    INSERT INTO [Movies] ([Code], [Title], [Director], [Year], [DurationMinutes], [Language], [Rating], [Seasons], [RentalCost], [Genres])
    VALUES
        (N'MX00000001', N'The Matrix', N'Wachowski Sisters', 1999, 136, N'English', 8.7, 0, 3.99, N'SciFi,Action'),
        (N'BR00000002', N'Breaking Bad', N'Vince Gilligan', 2008, 47, N'English', 9.5, 5, 2.49, N'Drama,Thriller'),
        (N'PS00000003', N'Parasite', N'Bong Joon Ho', 2019, 132, N'Korean', 8.6, 0, 4.49, N'Drama,Thriller');
END;
GO
