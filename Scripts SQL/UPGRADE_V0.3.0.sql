-- Script pour la base Ponant.Medical.Shore

-- Alter View vCruiseBoard
USE [Ponant.Medical.Shore]
GO
ALTER VIEW [dbo].[vCruiseBoard]
AS
SELECT
c.Id,
c.Code,
c.SailingDate,
c.SailingLengthDays,
ISNULL(CAST(COUNT_BIG(p.Id) AS int), 0) AS NbPassenger,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 30 OR p.IdStatus = 34 THEN 1 ELSE NULL END) AS int), 0) AS NbQMAvailable,
ISNULL(CAST(COUNT_BIG(CASE WHEN p.IdStatus = 33 THEN 1 ELSE NULL END) AS int), 0) AS NbQMDone,
c.Idship AS IdShip
FROM dbo.Cruise c
INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
INNER JOIN dbo.Passenger p ON bcp.IdPassenger = p.Id
WHERE CONVERT(date, c.SailingDate) >= DATEADD(DAY, -c.SailingLengthDays, CONVERT(date, GETDATE()))
GROUP BY c.Id, c.Code, c.SailingDate, c.SailingLengthDays,c.IdShip;
GO

-- Script pour la base Ponant.Medical.Auth

-- Alter Table AspNetUsers
USE [Ponant.Medical.Auth]
GO
IF NOT EXISTs(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'IdShip')
BEGIN
	ALTER TABLE dbo.AspNetUsers ADD IdShip Int NULL; 
END
GO

-- Alter View vUsers
USE [Ponant.Medical.Auth]
GO
ALTER VIEW [dbo].[vUsers]
AS
SELECT
u.Id,
u.LastName,
u.FirstName,
u.UserName Login,
u.Email,
u.Enabled,
r.Id AS RoleId,
r.Name AS Role,
lv.Name AS Ship
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN dbo.AspNetRoles r ON r.Id = ur.RoleId
LEFT OUTER JOIN [Ponant.Medical.Shore].dbo.Lov lv on lv.Id = u.IdShip
WHERE u.UserName <> 'System'
GO