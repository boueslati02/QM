USE [Ponant.Medical.Shore]
GO

-- Add new field Activity in CruiseCriterion Table
IF NOT EXISTs(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CruiseCriterion' AND COLUMN_NAME = 'Activity')
BEGIN
	ALTER TABLE CruiseCriterion ADD Activity varchar(130) NULL;
END
GO

-- Drop CruiseCriterionActivity table
DROP TABLE IF EXISTS CruiseCriterionActivity;
GO

-- Delete and create vCriteria view
DROP VIEW IF EXISTS [dbo].[vCriteria];
GO
CREATE VIEW [dbo].[vCriteria] AS
SELECT DISTINCT cc.Id AS 'Id', cc.[Order] AS 'Order', (CASE cc.IdCruiseType WHEN 0 THEN '' ELSE lov.Name END) AS 'CruiseType', (Substring
                             ((SELECT        ', ' + lov.Name
                                 FROM         dbo.CruiseCriterionDestination AS ccd INNER JOIN dbo.Lov AS lov ON lov.Id = ccd.IdDestination
                                 WHERE        ccd.IdCruiseCriterion = cc.Id
                                 ORDER BY ccd.IdCruiseCriterion, lov.Name FOR XML PATH('')), 3, 1000)) AS 'Destination', (Substring
                             ((SELECT        ', ' + lov.Name
                                 FROM         dbo.CruiseCriterionShip AS ccs INNER JOIN dbo.Lov AS lov ON lov.Id = ccs.IdShip
                                 WHERE        ccs.IdCruiseCriterion = cc.Id
                                 ORDER BY ccs.IdCruiseCriterion, ccs.IdShip FOR XML PATH('')), 3, 1000)) AS 'Ship', (cc.Length) AS 'Length',
								 cc.Cruise AS 'Cruise',
								 cc.Activity AS 'Activity',
								 sur.Name AS 'Survey',
								 cc.ModificationDate AS 'ModificationDate'
FROM					dbo.CruiseCriterion AS cc INNER JOIN
                        dbo.Lov AS lov ON lov.Id = cc.IdCruiseType INNER JOIN
                        dbo.Survey AS sur ON cc.IdSurvey = sur.Id
GO