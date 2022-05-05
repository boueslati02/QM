USE [Ponant.Medical.Shore]
GO

IF NOT EXISTs(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Passenger' AND COLUMN_NAME = 'AutoAttachment')
BEGIN
	ALTER TABLE Passenger ADD AutoAttachment BIT NULL;
END
GO

-- Delete and create vPassengerShore view
DROP VIEW IF EXISTS [dbo].[vPassengerShore];
GO
CREATE VIEW [dbo].[vPassengerShore] AS
SELECT DISTINCT 
p.Id, p.Number as 'PassengerNumber', CASE p.IdTitle WHEN 0 THEN NULL ELSE lovT.[Name] END AS 'Civility', p.LastName AS 'LastName', 
p.FirstName AS 'FirstName', p.Email AS 'Email', p.IsEmailUpdated AS 'EmailUpdated', p.IsEmailValid AS 'ValidEmail', p.PhoneNumber AS 'PhoneNumber', 
p.IdStatus AS 'IdStatus', CASE p.IdStatus WHEN 0 THEN NULL ELSE lovS.[Name] END AS 'QMStatus', p.SentDate AS 'SendingDate',
p.SentCount AS 'NbSent', 
p.IdAdvice AS 'IdAdvice', CASE ISNULL(p.IdAdvice, 0) WHEN 0 THEN NULL ELSE lovA.[Name] END AS 'Advice',
p.Doctor AS 'Doctor', p.TreatmentDate AS 'TreatmentDate', bcp.IsEnabled AS 'IsEnabled', p.Review AS 'Review',
p.IsExtract AS 'IsExtract',
p.AutoAttachment AS 'AutoAttachment',

CASE ISNULL(b.IdAgency, 0) WHEN 0 THEN NULL ELSE a.[Name] END AS 'BookingAgency',
b.Number AS 'BookingNumber', lovO.[Name] AS 'BookingOffice', b.IdOffice AS 'BookingIdOffice',
b.Id AS 'IdBooking', b.GroupName AS 'GroupName', b.IsGroup AS 'IsGroup', 
bcp.IdCruise AS 'IdCruise', c.Code AS 'CruiseCode'

FROM dbo.Passenger AS p
INNER JOIN dbo.BookingCruisePassenger AS bcp ON p.Id = bcp.IdPassenger
INNER JOIN dbo.Cruise AS c ON bcp.IdCruise = c.Id
INNER JOIN dbo.Booking AS b ON bcp.IdBooking = b.Id
INNER JOIN dbo.Lov AS LovA ON lovA.Id = p.IdAdvice
INNER JOIN dbo.Lov AS LovO ON lovO.Id = b.IdOffice
INNER JOIN dbo.Lov AS lovS ON lovS.Id = p.IdStatus
INNER JOIN dbo.Lov AS lovT ON lovT.Id = p.IdTitle
INNER JOIN dbo.Agency AS a ON b.IdAgency = a.Id
WHERE p.Id > 0
GO

-- Delete and create function AutoAttachmentNb
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AutoAttachmentNb]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN
	DROP FUNCTION [dbo].[AutoAttachmentNb]
END
GO
CREATE FUNCTION [dbo].[AutoAttachmentNb]
(
	@startDate datetime,
	@endDate datetime,
	@shipId int,
	@cruiseId int,
	@autoAttachment bit
)
RETURNS INT
AS
BEGIN
DECLARE @Nb INT

SELECT @Nb = COUNT(distinct p.Id)
FROM Passenger p
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdPassenger = p.Id
	INNER JOIN dbo.Cruise c ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Document as d ON p.Id = d.IdPassenger
WHERE bcp.IdCruise = ISNULL(@cruiseId, bcp.IdCruise)
	AND c.IdShip = ISNULL(@shipId, c.IdShip) -- Filtre cruise
	AND d.IdPassenger <> 0					 -- Document bien rataché
	AND (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
	AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)	-- Filtre date
	AND p.AutoAttachment = @autoAttachment
RETURN @Nb
END
GO

-- Create or Alter stored function AutoAttachmentFormula
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AutoAttachmentFormula]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN
	DROP FUNCTION [dbo].[AutoAttachmentFormula]
END
GO
CREATE FUNCTION [dbo].[AutoAttachmentFormula]
(
	@startDate datetime,
	@endDate datetime,
	@shipId int,
	@cruiseId int
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
DECLARE @Res DECIMAL(18, 2)

DECLARE @autoAttachmentTrue BIT
SET @autoAttachmentTrue = 1;

DECLARE @autoAttachmentFalse BIT
SET @autoAttachmentFalse = 0;

IF CAST(dbo.AutoAttachmentNb(@startDate, @endDate, @shipId, @cruiseId, @autoAttachmentTrue) + dbo.AutoAttachmentNb(@startDate, @endDate, @shipId, @cruiseId, @autoAttachmentFalse) AS DECIMAL) > 0
	SELECT @Res =
	CAST(ROUND(
	(
		CAST(dbo.AutoAttachmentNb(@startDate, @endDate, @shipId, @cruiseId, @autoAttachmentTrue) AS DECIMAL)
		/
		CAST(dbo.AutoAttachmentNb(@startDate, @endDate, @shipId, @cruiseId, @autoAttachmentTrue) + dbo.AutoAttachmentNb(@startDate, @endDate, @shipId, @cruiseId, @autoAttachmentFalse) AS DECIMAL)
		* 100
	), 2) AS DECIMAL(18, 2))
ELSE
	SELECT @Res = null
RETURN @Res
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
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @sentList) 'QmSent',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @receivedList) 'QmReceived',
	dbo.QmStatisticsStatus(@startDate, @endDate, c.IdShip, c.Id, @downloadList) 'QmDownload',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @treatedList) 'QmTreated',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @validatedList) 'QmValidated',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @refusedList) 'QmRefused',
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @waitingList) 'QmWaiting',
	dbo.AutoAttachmentFormula(@startDate, @endDate, c.IdShip, c.Id) 'AutoAttachment'
	FROM dbo.Cruise as c
	INNER JOIN dbo.Lov l ON l.Id = c.IdShip

END
GO