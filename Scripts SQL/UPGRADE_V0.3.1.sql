
-- Script pour la base Ponant.Medical.Shore

-- Alter View vPassengerBoard
USE [Ponant.Medical.Shore]
GO
ALTER VIEW [dbo].[vPassengerBoard] 
AS
SELECT DISTINCT
p.Id,
p.LastName,
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