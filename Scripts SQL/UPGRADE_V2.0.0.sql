-- Use Shore Database
USE [Ponant.Medical.Shore]
GO

-- Create AgencyAccessRight table
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'AgencyAccessRight'))
BEGIN    
	CREATE TABLE [dbo].[AgencyAccessRight] (
        [Id] INT IDENTITY(1, 1) NOT NULL,
		[IdAgency] INT NOT NULL,
        [CruiseCode] VARCHAR(16) NOT NULL,
		[GroupName] VARCHAR(32) NOT NULL,
		[BookingNumber] int NOT NULL,
		[Creator] VARCHAR(64) NOT NULL,
		[CreationDate] DATETIME NOT NULL,
		[Editor] VARCHAR(64) NOT NULL,
		[ModificationDate] DATETIME NOT NULL,
        CONSTRAINT PK_AgencyAccessRight PRIMARY KEY (Id)
    )
END
GO

IF (NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_AgencyAccessRight_Agency'))
BEGIN
	ALTER TABLE [dbo].[AgencyAccessRight] ADD CONSTRAINT [FK_AgencyAccessRight_Agency] FOREIGN KEY([IdAgency]) REFERENCES [dbo].[Agency] ([Id])
END
GO

IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Agency' AND COLUMN_NAME='Number') = 0)
BEGIN
	ALTER TABLE [dbo].[Agency] ADD Number INT NULL
END
GO

DROP VIEW IF EXISTS [dbo].[vAgencyAccessRight];
GO
CREATE VIEW [dbo].[vAgencyAccessRight]
AS
SELECT
	ara.[Id],
	ara.IdAgency as 'IdAgency',
	ISNULL(CONVERT(varchar, a.[Number]) + ' - ' + a.[Name], a.[Name]) as 'AgencyName',
	ara.[CruiseCode] as 'CruiseCode',
	ara.[GroupName] as 'GroupName',
	ara.[BookingNumber] as 'BookingNumber'
FROM [dbo].[AgencyAccessRight] ara
	INNER JOIN [dbo].[Agency] a ON a.Id = ara.IdAgency
GO


DROP VIEW IF EXISTS [dbo].[vPassengerShore];
GO
CREATE VIEW [dbo].[vPassengerShore]
AS
SELECT DISTINCT 
	p.Id,
	CASE p.IdTitle WHEN 0 THEN NULL ELSE lovT.[Name] END AS Civility, 
	p.LastName, 
	p.FirstName, 
	p.Email, 
	p.IsEmailUpdated AS EmailUpdated, 
	p.IsEmailValid AS ValidEmail,
	p.PhoneNumber,
	p.IdStatus, 
	CASE p.IdStatus WHEN 0 THEN NULL ELSE lovS.[Name] END AS QMStatus, 
	p.SentDate AS SendingDate,
	Max(d.ReceiptDate) AS ReceiptDate,
	p.SentCount AS NbSent, 
	p.IdAdvice, 
	CASE ISNULL(p.IdAdvice, 0) 
	WHEN 0 THEN NULL ELSE lovA.[Name] END AS Advice, 
	p.Doctor, 
	p.TreatmentDate, 
	bcp.IsEnabled, 
	p.Review, 
	p.IsExtract, 
	p.AutoAttachment, 
	p.UsualName,
	CASE ISNULL(b.IdAgency, 0) WHEN 0 THEN NULL ELSE a.[Name] END AS BookingAgency, 
	b.Number AS BookingNumber, 
	LovO.Name AS BookingOffice, 
	b.IdOffice AS BookingIdOffice, 
	b.Id AS IdBooking, 
	b.GroupName, 
	b.IsGroup, 
	bcp.IdCruise, 
	c.Code AS CruiseCode
FROM dbo.Passenger AS p INNER JOIN
	dbo.BookingCruisePassenger AS bcp ON p.Id = bcp.IdPassenger INNER JOIN
	dbo.Cruise AS c ON bcp.IdCruise = c.Id INNER JOIN
	dbo.Booking AS b ON bcp.IdBooking = b.Id INNER JOIN
	dbo.Lov AS LovA ON LovA.Id = p.IdAdvice INNER JOIN
	dbo.Lov AS LovO ON LovO.Id = b.IdOffice INNER JOIN
	dbo.Lov AS lovS ON lovS.Id = p.IdStatus INNER JOIN
	dbo.Lov AS lovT ON lovT.Id = p.IdTitle INNER JOIN
	dbo.Agency AS a ON b.IdAgency = a.Id LEFT JOIN
	dbo.Document as d ON d.IdPassenger = p.Id
WHERE (p.Id > 0)
GROUP BY p.Id, 
lovT.[Name],
p.LastName, 
p.FirstName,
p.Email,
p.IsEmailUpdated,
p.IsEmailValid,
p.PhoneNumber, 
p.IdStatus, 
lovS.[Name],
p.SentDate,
p.SentCount,
p.IdAdvice,
lovA.[Name],
p.Doctor,
p.TreatmentDate,
bcp.IsEnabled,
p.Review,
p.IsExtract,
p.AutoAttachment,
p.UsualName,
a.[Name],
b.Number,
LovO.[Name],
b.IdOffice,
b.Id,
b.GroupName,
b.IsGroup,
bcp.IdCruise, 
c.Code,
p.IdTitle,
b.IdAgency
GO

-- Delete and create stored procedure QmStatistics
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetPassengersGroup]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetPassengersGroup];
END;
GO
CREATE PROCEDURE [dbo].[GetPassengersGroup]
@IdBooking int
AS
BEGIN
SELECT 
	CASE p.IdTitle WHEN 0 THEN NULL ELSE lovTitle.[Name] END AS 'Civility',
	p.LastName AS 'LastName',
	p.UsualName AS 'UsualName',
	p.FirstName AS 'FirstName',
	CASE WHEN acr.Id IS NOT NULL THEN p.Email ELSE NULL END AS 'Email',
	CASE WHEN acr.Id IS NOT NULL THEN p.IsEmailUpdated ELSE NULL END AS 'EmailUpdated',
	p.IsEmailValid AS 'ValidUpdated',
	CASE p.IdStatus WHEN 0 THEN NULL ELSE lovStatus.[Name] END AS 'QmStatus', 
	p.SentDate AS 'SendingDate',
	Max(d.ReceiptDate) as 'ReceiptDate',
	p.SentCount AS 'NbSent',
	p.AutoAttachment AS 'AutoAttachment',
	CASE  WHEN acr.Id IS NULL THEN 'PONANT' ELSE a.[Name] END AS 'AgencyName',
	p.IdAdvice AS 'IdAdvice',
	c.Code AS 'CruiseCode',
	b.GroupName,
	b.Number AS 'BookingNumber',
	bcp.IsEnabled
FROM dbo.Booking b
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdBooking = b.Id
	INNER JOIN dbo.Passenger p ON p.Id = bcp.IdPassenger
	INNER JOIN dbo.Cruise c ON c.Id = bcp.IdCruise
	INNER JOIN dbo.Lov lovStatus ON lovStatus.Id = p.IdStatus
	INNER JOIN dbo.Lov lovTitle ON lovTitle.Id = p.IdTitle
	LEFT OUTER JOIN dbo.AgencyAccessRight acr ON acr.BookingNumber = b.Number
	LEFT OUTER JOIN dbo.Agency a ON a.Id = acr.IdAgency
	LEFT OUTER JOIN dbo.Document d on  p.Id = d.IdPassenger
WHERE p.Id > 0 
	AND b.Id = @IdBooking
GROUP BY p.IdTitle,
	lovTitle.[Name],
	p.LastName,
	p.UsualName,
	p.FirstName,
	acr.Id,
	p.Email,
	p.IsEmailUpdated,
	p.IsEmailValid,
	p.IdStatus,
	lovStatus.[Name], 
	p.SentDate,
	p.SentCount,
	p.AutoAttachment,
	a.[Name],
	p.IdAdvice,
	c.Code,
	b.GroupName,
	b.Number,
	bcp.IsEnabled
END
GO

DROP VIEW IF EXISTS [dbo].[vCruiseShore];
GO
CREATE VIEW [dbo].[vCruiseShore]
AS
SELECT cr.Id AS 'Id', cr.Code AS 'Code', 
CASE cr.IdTypeCruise WHEN 0 THEN NULL ELSE cr.IdTypeCruise END AS 'IdTypeCruise',
CASE cr.IdTypeCruise WHEN 0 THEN NULL ELSE lovTC.[Name] END AS 'TypeCruise',
cr.SailingDate AS 'DateDeparture',
CASE cr.IdDestination WHEN 0 THEN NULL ELSE cr.IdDestination END AS 'IdDestination',
CASE cr.IdDestination WHEN 0 THEN NULL ELSE lovD.[Name] END AS 'Destination',
CASE cr.IdShip WHEN 0 THEN NULL ELSE cr.IdShip END AS 'IdShip',
CASE cr.IdShip WHEN 0 THEN NULL ELSE lovS.[Name] END AS 'Ship',
cr.IsExtract AS 'IsExtract',
(Substring
((SELECT DISTINCT ', ' +  CASE b.IdAgency WHEN 0 THEN NULL ELSE ISNULL(CONVERT(varchar, a.[Number]) + ' - ' + a.[Name], a.[Name]) END
FROM dbo.BookingCruisePassenger AS bcp
INNER JOIN dbo.Booking AS b ON bcp.IdBooking = b.Id
INNER JOIN dbo.Agency AS a ON b.IdAgency = a.Id
WHERE bcp.IdCruise = cr.Id
FOR XML PATH('')), 3, 1000)) AS 'Agency'
FROM dbo.Cruise AS cr
INNER JOIN dbo.Lov AS lovTC ON cr.IdTypeCruise = lovTC.Id
INNER JOIN dbo.Lov AS lovD ON cr.IdDestination = lovD.Id
INNER JOIN dbo.Lov AS lovS ON cr.IdShip = lovS.Id
GO

IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Passenger' AND COLUMN_NAME='Token') = 0)
BEGIN
	ALTER TABLE [dbo].[Passenger] ADD Token VARCHAR(36) NULL;
END
GO

USE [Ponant.Medical.Auth]
GO

IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'AspNetUsers' AND COLUMN_NAME='IdAgency') = 0)
BEGIN
	ALTER TABLE [dbo].[AspNetUsers] ADD IdAgency INT NULL;

	UPDATE [dbo].[AspNetUsers] SET IdAgency = 135;

	ALTER TABLE [dbo].[AspNetUsers] ALTER COLUMN IdAgency INT NOT NULL;
END
GO

IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'AspNetUsers' AND COLUMN_NAME='LogoName') = 0)
BEGIN
	ALTER TABLE [dbo].[AspNetUsers] ADD LogoName VARCHAR(128) NULL;
END
GO

DROP VIEW IF EXISTS [dbo].[vUsers];
GO
CREATE VIEW [dbo].[vUsers]
AS
SELECT
	u.Id, 
	u.LastName, 
	u.FirstName, 
	u.UserName 'Login',
	u.Email, 
	u.[Enabled], 
	r.Id 'RoleId', 
	r.[Name] 'Role', 
	lv.Id 'IdShip',
	lv.[Name] 'Ship',
	a.Id 'IdAgency',
	ISNULL(CONVERT(varchar, a.[Number]) + ' - ' + a.[Name], a.[Name]) 'AgencyName',
	u.LogoName
FROM
	dbo.AspNetUsers u
	INNER JOIN dbo.AspNetUserRoles ur ON u.Id = ur.UserId 
	INNER JOIN dbo.AspNetRoles r ON r.Id = ur.RoleId 
	LEFT OUTER JOIN	[Ponant.Medical.Shore].[dbo].Lov lv ON lv.Id = u.IdShip 
	INNER JOIN [Ponant.Medical.Shore].[dbo].[Agency] a ON a.Id = u.IdAgency
WHERE	(u.UserName <> 'System')
GO

IF (NOT EXISTS(SELECT * FROM AspNetRoles WHERE [Name] = 'Agency'))
BEGIN
	INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) VALUES ('AFBAD3B9-124B-4943-9221-25A11437A4D3','Agency')
END
GO

IF (NOT EXISTS(SELECT * FROM AspNetRoles WHERE [Name] = 'Agency Administrator'))
BEGIN
INSERT INTO [dbo].[AspNetRoles] ([Id],[Name]) VALUES('5F2BBA55-37C6-4179-BC08-8E10F579E9DF','Agency Administrator')
END
GO