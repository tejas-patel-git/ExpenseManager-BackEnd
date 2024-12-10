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
CREATE TABLE [User] (
    [UserID] int NOT NULL IDENTITY,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Phone] nvarchar(15) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([UserID])
);

CREATE TABLE [Transactions] (
    [TransactionID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [IsExpense] bit NOT NULL,
    [Amount] decimal(10,2) NOT NULL,
    [Date] datetime2 NOT NULL,
    [Description] nvarchar(500) NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([TransactionID]),
    CONSTRAINT [FK_Transactions_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [User] ([UserID]) ON DELETE CASCADE
);

CREATE INDEX [IX_Transactions_UserID] ON [Transactions] ([UserID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241209190141_InitialMigration', N'9.0.0');

COMMIT;
GO

