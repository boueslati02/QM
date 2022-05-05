USE [Ponant.Medical.Shore]
GO

-- Create Assignment table
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Assignment'))
BEGIN    
	CREATE TABLE [dbo].[Assignment] (
        [Id] INT IDENTITY(1, 1) NOT NULL,
		[IdShip] INT NOT NULL,
        [Cruises] VARCHAR(130) NOT NULL,
		[Deadline] DATETIME NOT NULL,
		[Creator] VARCHAR(64) NOT NULL,
		[CreationDate] DATETIME NOT NULL,
		[Editor] VARCHAR(64) NOT NULL,
		[ModificationDate] DATETIME NOT NULL,
        CONSTRAINT PK_Assignment PRIMARY KEY (Id),
		CONSTRAINT FK_IdShip FOREIGN KEY ([IdShip]) REFERENCES [dbo].[Lov] ([Id])
    )
END
GO

IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Passenger' AND COLUMN_NAME='IsDownloaded') = 0)
BEGIN
	ALTER TABLE [dbo].[Passenger] ADD IsDownloaded bit NULL
END
GO

DROP VIEW IF EXISTS [dbo].[vPassengerBoard];
GO
CREATE VIEW [dbo].[vPassengerBoard] 
AS
SELECT DISTINCT
		p.Id,
		p.LastName,
		p.UsualName,
		p.FirstName,
		p.Email,
		p.IdAdvice,
		p.Review,
		p.IdStatus,
		p.IsExtract,
		p.IsDownloaded, 
		u.LastName 'Doctor',
		bcp.IdCruise,
		(SELECT TOP 1 d.ReceiptDate FROM dbo.Document d WHERE d.IdPassenger = p.Id AND d.ReceiptDate IS NOT NULL ORDER BY d.ReceiptDate desc) AS ReceiptDate
FROM dbo.Passenger p
INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdPassenger = p.Id LEFT JOIN
	[Ponant.Medical.Auth].dbo.AspNetUsers AS u ON u.Id = p.Doctor
GO

IF ((SELECT Count(*) from [dbo].[Lov] WHERE [Id] = 35 AND [IsEnabled] = 1) = 1)
BEGIN
	UPDATE [dbo].[Passenger] SET [IsDownloaded] = 0 WHERE [IdStatus] <> 35
	UPDATE [dbo].[Passenger] SET [IsDownloaded] = 1 WHERE [IdStatus] = 35
	ALTER TABLE [dbo].[Passenger] ALTER COLUMN [IsDownloaded] bit NOT NULL
	UPDATE [dbo].[Lov] SET [IsEnabled] = 0 WHERE [Id] = 35
END
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
	p.Doctor 'DoctorId', 
	u.LastName 'Doctor',
	p.TreatmentDate, 
	bcp.IsEnabled, 
	p.Review, 
	p.IsExtract, 
	p.AutoAttachment, 
	p.UsualName,
	p.IsDownloaded,
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
	dbo.Document as d ON d.IdPassenger = p.Id LEFT JOIN
	[Ponant.Medical.Auth].dbo.AspNetUsers AS u ON u.Id = p.Doctor
WHERE (p.Id > 0)
GROUP BY 
	p.Id, 
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
	u.LastName,
	u.FirstName,
	p.TreatmentDate,
	bcp.IsEnabled,
	p.Review,
	p.IsExtract,
	p.AutoAttachment,
	p.UsualName,
	p.IsDownloaded,
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

DROP VIEW IF EXISTS [dbo].[vCruiseBoard];
GO
CREATE VIEW [dbo].[vCruiseBoard]
AS
SELECT
c.Id,
c.Code,
c.SailingDate,
c.SailingLengthDays,
ISNULL(CAST(COUNT_BIG(DISTINCT p.Id) AS int), 0) AS NbPassenger,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 30 OR p.IdStatus = 32 OR p.IdStatus = 34 THEN 1 ELSE NULL END) AS int), 0) AS NbQMAvailable,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 33 THEN 1 ELSE NULL END) AS int), 0) AS NbQMDone,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 28 OR p.IdStatus = 29 OR p.IdStatus = 31 THEN 1 ELSE NULL END) AS int), 0) AS NbQMNotAvailable,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 30 OR p.IdStatus = 33 OR p.IdStatus = 32 OR p.IdStatus = 34  THEN 1 ELSE NULL END) AS int), 0) AS NbQMReceive,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdAdvice = 44 OR p.IdAdvice = 45 THEN 1 ELSE NULL END) AS int), 0) AS NbQMValidate,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdAdvice = 47 THEN 1 ELSE NULL END) AS int), 0) AS NbQMWaiting,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdAdvice = 46 THEN 1 ELSE NULL END) AS int), 0) AS NbQMRefused,
c.Idship AS IdShip,
a.Deadline,
a.IdShip AS IdShipAssigned
FROM dbo.Cruise c
INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
INNER JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
LEFT JOIN dbo.Assignment a ON a.Cruises LIKE '%' + c.Code + '%'
LEFT JOIN dbo.Lov s ON s.Id = a.IdShip
WHERE CONVERT(date, c.SailingDate) >= DATEADD(DAY, -c.SailingLengthDays, CONVERT(date, GETDATE()))
AND bcp.IsEnabled = 1
GROUP BY c.Id, c.Code, c.SailingDate, c.SailingLengthDays, c.IdShip, a.Deadline, a.IdShip;
GO

IF ((SELECT Count(*) from [dbo].[Lov] WHERE [Name] = 'No vaccine') = 0)
BEGIN
	INSERT INTO [dbo].[Lov]
			   ([IdLovType]
			   ,[Code]
			   ,[Name]
			   ,[IsEnabled]
			   ,[Creator]
			   ,[CreationDate]
			   ,[Editor]
			   ,[ModificationDate])
		 VALUES
			   (12
			   ,NULL
			   ,'No vaccine'
			   ,1
			   ,'System'
			   ,GETDATE()
			   ,'System'
			   ,GETDATE())
END
GO

-- Delete and create stored procedure QmStatistics
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[QmStatistics]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[QmStatistics];
END;
GO
CREATE PROCEDURE [dbo].[QmStatistics]
	-- Add the parameters for the stored procedure here
	@startDate datetime,
	@endDate datetime
AS
BEGIN
DECLARE @allList IntList
INSERT INTO @allList VALUES (28)
INSERT INTO @allList VALUES (29)
INSERT INTO @allList VALUES (30)
INSERT INTO @allList VALUES (31)
INSERT INTO @allList VALUES (32)
INSERT INTO @allList VALUES (33)
INSERT INTO @allList VALUES (34)
INSERT INTO @allList VALUES (35)

DECLARE @sentList IntList
INSERT INTO @sentList VALUES (28)
INSERT INTO @sentList VALUES (30)
INSERT INTO @sentList VALUES (33)
INSERT INTO @sentList VALUES (32)
INSERT INTO @sentList VALUES (34)
INSERT INTO @sentList VALUES (35)

DECLARE @receivedList IntList
INSERT INTO @receivedList VALUES (30)
INSERT INTO @receivedList VALUES (33)
INSERT INTO @receivedList VALUES (32)
INSERT INTO @receivedList VALUES (34)
INSERT INTO @receivedList VALUES (35)

DECLARE @downloadList IntList
INSERT INTO @downloadList VALUES (35)

DECLARE @treatedList IntList
INSERT INTO @treatedList VALUES (44)
INSERT INTO @treatedList VALUES (45)
INSERT INTO @treatedList VALUES (46)

DECLARE @validatedList IntList
INSERT INTO @validatedList VALUES (44)
INSERT INTO @validatedList VALUES (45)

DECLARE @refusedList IntList
INSERT INTO @refusedList VALUES (46)

DECLARE @waitingList IntList
INSERT INTO @waitingList VALUES (47)

	SELECT DISTINCT
	c.IdShip,
	l.[Name] 'ShipName',
	c.Id 'IdCruise',
	c.Code 'CruiseCode',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @allList) 'QmAll',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @sentList) 'QmSent',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @receivedList) 'QmReceived',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @downloadList) 'QmDownload',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @treatedList) 'QmTreated',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @validatedList) 'QmValidated',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @refusedList) 'QmRefused',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @waitingList) 'QmWaiting',
	dbo.AutoAttachmentFormula(@startDate, @endDate, c.IdShip, c.Id) 'AutoAttachment',
	s.[Name] 'AssignedShip',
	a.Deadline
	FROM dbo.Cruise as c
	INNER JOIN dbo.Lov l ON l.Id = c.IdShip
	LEFT JOIN dbo.Assignment a ON a.Cruises LIKE '%' + c.Code + '%'
	LEFT JOIN dbo.Lov s ON s.Id = a.IdShip

END
GO

-- Delete and create stored procedure QmTreatment
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[QmTreatment]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[QmTreatment];
END;
GO
CREATE PROCEDURE [dbo].[QmTreatment]
	@year int,
	@month int
AS
BEGIN
	SELECT DISTINCT
	c.IdShip,
	l.[Name] 'ShipName',
	c.Id 'IdCruise',
	c.Code 'CruiseCode',
	p.LastName,
	p.UsualName,
	p.FirstName,
	p.TreatmentDate,
	(SELECT COUNT(Id) FROM dbo.BookingCruisePassenger WHERE IsEnabled = 1 AND IdCruise = c.Id) 'TotalPassenger'
	FROM dbo.Cruise as c
	INNER JOIN dbo.Lov l ON l.Id = c.IdShip
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id AND bcp.IsEnabled = 1
	INNER JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
	WHERE YEAR(p.TreatmentDate) = @year
	AND MONTH(p.TreatmentDate) = @month
END
GO

-- Delete and create stored procedure GetShipsTreatment
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetShipsTreatment]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetShipsTreatment];
END;
GO
CREATE PROCEDURE [dbo].[GetShipsTreatment]
@year int,
@month int
AS
BEGIN
	SELECT DISTINCT l.id, l.[Name]
	FROM dbo.Cruise c
	INNER JOIN dbo.Lov l ON c.IdShip = l.Id
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
	WHERE YEAR(p.TreatmentDate) = @year
	AND MONTH(p.TreatmentDate) = @month
	ORDER BY l.[Name]
END
GO

-- Delete and create stored procedure GetCruisesTreatment
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetCruisesTreatment]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetCruisesTreatment];
END;
GO
CREATE PROCEDURE [dbo].[GetCruisesTreatment]
@year int,
@month int
AS
BEGIN
	SELECT DISTINCT c.Id, c.Code, c.IdShip
	FROM dbo.Cruise c
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
	WHERE YEAR(p.TreatmentDate) = @year
	AND MONTH(p.TreatmentDate) = @month
	ORDER BY c.Code
END
GO
GO

DROP VIEW IF EXISTS [dbo].[vAssignment];
GO
CREATE VIEW [dbo].[vAssignment]
AS
  SELECT
	  a.Id,
	  a.[IdShip],
	  l.[Name] as 'Ship',
      a.[Cruises],
      a.[Deadline]
  FROM [Ponant.Medical.Shore].[dbo].[Assignment] a
  INNER JOIN [dbo].[Lov] l ON a.[IdShip] = l.[Id]
GO

-- Préparation reprise de données
select distinct p.doctor, u.UserName, u.Id
from Passenger p
left join [Ponant.Medical.Auth].dbo.AspNetUsers u on u.UserName = p.Doctor
GO

UPDATE Passenger SET Doctor = '07ca7436-cec3-4b08-91f1-f8229e339a62' WHERE Doctor = 'drosenbaum'
UPDATE Passenger SET Doctor = 'bd208bf2-84fa-4652-9df3-3ff1b06dc960' WHERE Doctor = 'doctor.soleal'
UPDATE Passenger SET Doctor = 'f3720841-a291-4430-a6d3-6e37bf89c70d' WHERE Doctor = 'doctor.lyrial'
UPDATE Passenger SET Doctor = '07ca7436-cec3-4b08-91f1-f8229e339a62' WHERE Doctor = 'EG'
UPDATE Passenger SET Doctor = 'f3720841-a291-4430-a6d3-6e37bf89c70d' WHERE Doctor = 'LY'
UPDATE Passenger SET Doctor = '32ced21e-bb46-478c-b8cb-a5449fa39977' WHERE Doctor = 'doctor.boreal'
UPDATE Passenger SET Doctor = '32ced21e-bb46-478c-b8cb-a5449fa39977' WHERE Doctor = 'boreal'
UPDATE Passenger SET Doctor = 'a5226287-09bd-4c28-990f-bd3e69db2c61' WHERE Doctor = 'EU'
UPDATE Passenger SET Doctor = '5e27ee81-331c-460f-b479-de57d4ae6c20' WHERE Doctor = 'doctor.austral'
UPDATE Passenger SET Doctor = '5e27ee81-331c-460f-b479-de57d4ae6c20' WHERE Doctor = 'AU'
UPDATE Passenger SET Doctor = 'bd208bf2-84fa-4652-9df3-3ff1b06dc960' WHERE Doctor = 'SO'
UPDATE Passenger SET Doctor = 'bd208bf2-84fa-4652-9df3-3ff1b06dc960' WHERE Doctor = 'soleal'
UPDATE Passenger SET Doctor = 'e8f66381-49cd-4f51-8081-e5232bc9607f' WHERE Doctor = 'EC'
UPDATE Passenger SET Doctor = '32ced21e-bb46-478c-b8cb-a5449fa39977' WHERE Doctor = 'BO'
GO

DROP VIEW IF EXISTS [dbo].[vSurvey];
GO
CREATE VIEW [dbo].[vSurvey] AS
SELECT        sur.Id AS 'Id', sur.[Name] AS 'Name',
                             (SELECT        COUNT(lang.Id)
                               FROM            dbo.[Language] AS lang
                               WHERE        lang.IdSurvey = sur.Id AND ISNULL(lang.IndividualSurveyFileName, '') <> '') AS 'NbIndividualSurveys', 				   
							     (SELECT        COUNT(lang.Id)
                               FROM            dbo.[Language] AS lang
                               WHERE        lang.IdSurvey = sur.Id AND ISNULL(lang.GroupSurveyFileName, '') <> '') AS 'NbGroupSurveys', (Substring
                             ((SELECT        ', ' + lov.Name
                                 FROM            dbo.[Language] AS lang LEFT JOIN dbo.Lov AS lov ON lov.Id = lang.IdLanguage
                                 WHERE        lang.IdSurvey = sur.Id
                                 ORDER BY lang.IdSurvey, lov.Name FOR XML PATH('')), 3, 1000)) AS 'Languages',
                             (SELECT        COUNT(lang.Id)
                               FROM            dbo.[Language] AS lang
                               WHERE        lang.IdSurvey = sur.Id AND ISNULL(lang.IndividualSurveyMail, '') <> '') AS 'NbIndividualSurveyMail',
                             (SELECT        COUNT(lang.Id)
                               FROM            dbo.[Language] AS lang
                               WHERE        lang.IdSurvey = sur.Id AND ISNULL(lang.GroupSurveyMail, '') <> '') AS 'NbGroupSurveyMail',
							   sur.MedicalAdvice AS 'MedicalAdvice'
FROM            Survey AS sur
GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[QmStatisticsStatus]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[QmStatisticsStatus]
  GO

CREATE FUNCTION [dbo].[QmStatisticsStatus]
(
	@startDate datetime,
	@endDate datetime,
	@shipId int,
	@cruiseId int,
	@lovId [dbo].[IntList] readonly	-- filtre de la data
)
RETURNS INT
AS
BEGIN
DECLARE @Nb INT

SELECT @Nb = COUNT(distinct p.Id)
FROM Passenger p
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdPassenger = p.Id
	INNER JOIN dbo.Cruise c ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Document as d ON p.Id = d.IdPassenger AND d.IdPassenger <> 0 -- Document bien rataché
		AND (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
		AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)	-- Filtre date
	INNER JOIN @lovId l ON p.IdStatus = l.Id
WHERE bcp.IdCruise = ISNULL(@cruiseId, bcp.IdCruise) 
	AND c.IdShip = ISNULL(@shipId, c.IdShip) -- Filtre cruise
RETURN @Nb
END
GO

USE [Ponant.Medical.Auth]
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name])
     VALUES
           (NEWID()
           ,'Medical Administrator')
GO