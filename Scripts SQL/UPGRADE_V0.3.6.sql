USE [Ponant.Medical.Shore]
GO

-- Add new field UsualName in Passenger Table
IF NOT EXISTs(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Passenger' AND COLUMN_NAME = 'UsualName')
BEGIN
	ALTER TABLE Passenger ADD UsualName varchar(64) NULL;
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
p.UsualName AS 'UsualName',

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

-- Delete and create vPassengerBoard view
DROP VIEW IF EXISTS [dbo].[vPassengerBoard];
GO
CREATE VIEW [dbo].[vPassengerBoard] 
AS
SELECT DISTINCT
p.Id,
p.LastName,
p.UsualName,
p.FirstName,
p.IdAdvice,
p.Review,
p.IdStatus,
p.IsExtract,
p.Doctor,
bcp.IdCruise,
(SELECT TOP 1 d.ReceiptDate FROM dbo.Document d WHERE d.IdPassenger = p.Id AND d.ReceiptDate IS NOT NULL ORDER BY d.ReceiptDate desc) AS ReceiptDate
FROM dbo.Passenger p
INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdPassenger = p.Id

GO