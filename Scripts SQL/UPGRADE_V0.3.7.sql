USE [Ponant.Medical.Shore]
GO

-- Delete and create vCruiseBoard view
DROP VIEW IF EXISTS [dbo].[vCruiseBoard];
GO
CREATE VIEW [dbo].[vCruiseBoard]
AS
SELECT DISTINCT
c.Id,
c.Code,
c.SailingDate,
c.SailingLengthDays,

--ISNULL(CAST(COUNT_BIG(p.Id) AS int), 0) AS NbPassenger,
ISNULL(CAST(COUNT_BIG(DISTINCT p.Id) AS int), 0) AS NbPassenger,

--ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 30 OR p.IdStatus = 34 THEN 1 ELSE NULL END) AS int), 0) AS NbQMAvailable,
ISNULL(
	(SELECT COUNT(Distinct p1.Id) 
	FROM Passenger p1 
	LEFT JOIN BookingCruisePassenger bcp1 ON bcp1.IdPassenger = p1.Id 
	WHERE bcp1.IdCruise = c.Id AND (p1.IdStatus = 30 OR p1.IdStatus = 34))
, 0) AS NbQMAvailable,

--ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 33 THEN 1 ELSE NULL END) AS int), 0) AS NbQMDone,
ISNULL(
	(SELECT COUNT(Distinct p2.Id) 
	FROM Passenger p2 
	LEFT JOIN BookingCruisePassenger bcp2 ON bcp2.IdPassenger = p2.Id 
	WHERE bcp2.IdCruise = c.Id AND (p2.IdAdvice = 44 OR p2.IdAdvice = 45 OR p2.IdAdvice = 46))
, 0)  AS NbQMDone,

c.Idship AS IdShip
FROM dbo.Cruise c
INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
INNER JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
WHERE CONVERT(date, c.SailingDate) >= DATEADD(DAY, -c.SailingLengthDays, CONVERT(date, GETDATE()))
GROUP BY c.Id, c.Code, c.SailingDate, c.SailingLengthDays, c.IdShip;
GO

