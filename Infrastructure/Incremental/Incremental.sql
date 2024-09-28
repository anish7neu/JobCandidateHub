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
GO

CREATE TABLE [Candidates] (
    [Id] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Email] nvarchar(100) NULL,
    [PreferredCallTime] datetime2 NOT NULL,
    [LinkedInProfileUrl] nvarchar(200) NULL,
    [GitHubProfileUrl] nvarchar(200) NULL,
    [Comment] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_Candidates] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240928051118_Initial', N'8.0.3');
GO

COMMIT;
GO

