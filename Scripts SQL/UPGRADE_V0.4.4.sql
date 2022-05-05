-- Use Auth Database
USE [Ponant.Medical.Auth]
GO

-- Create AspNetUserShips table
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'AspNetUserShips'))
BEGIN    
	CREATE TABLE [dbo].[AspNetUserShips] (
        [IdUser] [nvarchar](128) NOT NULL,
        [IdShip] [int] NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUserShips] PRIMARY KEY ([IdUser], [IdShip])
    )

    CREATE INDEX [IX_IdUser] ON [dbo].[AspNetUserShips]([IdUser])
    ALTER TABLE [dbo].[AspNetUserShips] 
		ADD CONSTRAINT [FK_dbo.AspNetUserShips_dbo.AspNetUsers_IdUser] 
		FOREIGN KEY ([IdUser]) REFERENCES [dbo].[AspNetUsers] ([Id])
END
GO

-- Init AspNetUserShips data

-- JACQUES CARTIER
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'f2cd305a-71de-4065-b8d3-b7c70639c28c';
DECLARE @IdShipValue int SET @IdShipValue = 376;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 569;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- L'AUSTRAL
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = '5e27ee81-331c-460f-b479-de57d4ae6c20';
DECLARE @IdShipValue int SET @IdShipValue = 23;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 563;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE BELLOT
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'c835b63c-f559-4ce2-b4ed-1afbee12685b';
DECLARE @IdShipValue int SET @IdShipValue = 331;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 608;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE BOREAL
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = '32ced21e-bb46-478c-b8cb-a5449fa39977';
DECLARE @IdShipValue int SET @IdShipValue = 24;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 607;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE BOUGAINVILLE
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = '20818754-9dc5-45ad-8ba5-863763f9ffd9';
DECLARE @IdShipValue int SET @IdShipValue = 323;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 566;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE CHAMPLAIN
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'e8f66381-49cd-4f51-8081-e5232bc9607f';
DECLARE @IdShipValue int SET @IdShipValue = 232;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 567;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE DUMONT D'URVILLE
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'ba4d7c54-124e-40d1-8b45-1e58174d664e';
DECLARE @IdShipValue int SET @IdShipValue = 348;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 565;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE LAPEROUSE
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'e9f6eb01-bf71-4ed2-9df7-43dc709ff7e6';
DECLARE @IdShipValue int SET @IdShipValue = 238;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE LYRIAL
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'f3720841-a291-4430-a6d3-6e37bf89c70d';
DECLARE @IdShipValue int SET @IdShipValue = 25;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 568;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE PONANT
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'e9c8df5f-4886-463d-94c4-0ecd712de0b6';
DECLARE @IdShipValue int SET @IdShipValue = 26;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- LE SOLEAL
DECLARE @IdUserValue nvarchar(128) SET @IdUserValue = 'bd208bf2-84fa-4652-9df3-3ff1b06dc960';
DECLARE @IdShipValue int SET @IdShipValue = 27;
UPDATE [dbo].[AspNetUsers] SET [IdShip] = @IdShipValue WHERE [Id] = @IdUserValue
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
SET @IdShipValue = 616;
IF NOT EXISTS (SELECT IdUser FROM dbo.AspNetUserShips WHERE IdUser = @IdUserValue AND IdShip = @IdShipValue)
BEGIN
    INSERT INTO [dbo].[AspNetUserShips]([IdUser], [IdShip]) VALUES(@IdUserValue, @IdShipValue);
END
GO

-- Use Shore Database
USE [Ponant.Medical.Shore]
GO

-- Update Existing Cruises
UPDATE [dbo].[Cruise] 
SET IdShip = u.idShip
FROM [Ponant.Medical.Auth].[dbo].[AspNetUsers] u
	INNER JOIN [Ponant.Medical.Auth].[dbo].[AspNetUserShips] us ON us.IdUser = u.Id
	INNER JOIN [dbo].[Cruise] c ON c.IdShip = us.IdShip
WHERE  u.IdShip IS NOT NULL
GO

-- Disable Ship Lov
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'AV';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'EW';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'EZ';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'EM';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'LT';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'EQ';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'BH';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'EF';
UPDATE [dbo].[Lov] SET IsEnabled = 0 WHERE IdLovType = 3 AND Code = 'SI';
GO