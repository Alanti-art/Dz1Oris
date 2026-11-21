IF DB_ID(N'CardGame') IS NULL
BEGIN
    CREATE DATABASE [CardGame];
END
GO

USE [CardGame];
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Login NVARCHAR(100) NOT NULL,
        Password NVARCHAR(200) NOT NULL,
        Role INT NOT NULL CONSTRAINT DF_Users_Role DEFAULT (0)
    );
END
GO

IF OBJECT_ID(N'dbo.Characters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Characters (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Image NVARCHAR(500) NULL,
        Description NVARCHAR(MAX) NULL,
        Action NVARCHAR(1000) NULL,
        Cost NVARCHAR(500) NULL,
        Blocks NVARCHAR(500) NULL,
        BlockedBy NVARCHAR(500) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Comments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Comments (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Text NVARCHAR(2000) NOT NULL,
        Rating NVARCHAR(10) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Comments_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO

IF OBJECT_ID(N'dbo.Rules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Rules (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_Rules_SortOrder DEFAULT (0)
    );
END
GO
