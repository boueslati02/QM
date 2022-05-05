USE [Ponant.Medical.Shore]
GO

-- Delete and create stored procedure GetCruises
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetCruises]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetCruises];
END;
GO
CREATE PROCEDURE [dbo].[GetCruises]
@startDate datetime,
@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT c.Id, c.Code, c.IdShip
	FROM dbo.Cruise c
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Document as d ON bcp.IdPassenger = d.IdPassenger
	WHERE (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
	AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)	-- Filtre date
	ORDER BY c.Code
END
GO

-- Delete and create stored procedure GetShips
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetShips]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetShips];
END;
GO
CREATE PROCEDURE [dbo].[GetShips]
@startDate datetime,
@endDate datetime
AS
BEGIN
	SELECT DISTINCT l.id, l.[Name]
	FROM dbo.Cruise c
	INNER JOIN dbo.Lov l ON c.IdShip = l.Id
	INNER JOIN dbo.BookingCruisePassenger bcp ON bcp.IdCruise = c.Id
	LEFT JOIN dbo.Document as d ON bcp.IdPassenger = d.IdPassenger
	WHERE (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
	AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)	-- Filtre date
	ORDER BY l.[Name]
END
GO

-- Drop stored procedur QmStatisticsStatus if exist
IF object_id('QmStatisticsStatus', 'fn') IS NOT NULL
BEGIN
	DROP FUNCTION [dbo].[QmStatisticsStatus];
END;
GO

-- Drop stored procedur [QmStatisticsAdvice] if exist
IF object_id('QmStatisticsAdvice', 'fn') IS NOT NULL
BEGIN
	DROP FUNCTION [dbo].[QmStatisticsAdvice];
END;
GO

-- Drop Create the type for sub-requests parameter
IF type_id('[dbo].[IntList]') IS NOT NULL
BEGIN
	DROP TYPE dbo.[IntList];
END
GO
CREATE TYPE IntList AS TABLE
(
   Id INT
)
GO

/****** Object:  UserDefinedFunction [dbo].[QmStatisticsStatus]    Script Date: 18/01/2019 10:42:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- create stored procedure ps_Report_DayByDay_Scenario
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
	LEFT JOIN dbo.Document as d ON p.Id = d.IdPassenger
	INNER JOIN @lovId l ON p.IdStatus = l.Id
WHERE bcp.IdCruise = ISNULL(@cruiseId, bcp.IdCruise) 
	AND c.IdShip = ISNULL(@shipId, c.IdShip) -- Filtre cruise
	AND d.IdPassenger <> 0										-- Document bien rataché
	AND (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
	AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)	-- Filtre date
RETURN @Nb
END
GO


/****** Object:  UserDefinedFunction [dbo].[QmStatisticsAdvice]    Script Date: 18/01/2019 11:23:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- create stored procedure [QmStatisticsAdvice]
CREATE FUNCTION [dbo].[QmStatisticsAdvice]
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
	LEFT JOIN dbo.Document as d ON p.Id = d.IdPassenger
	INNER JOIN @lovId l ON p.IdAdvice = l.Id
WHERE bcp.IdCruise = ISNULL(@cruiseId, bcp.IdCruise)
	AND c.IdShip = ISNULL(@shipId, c.IdShip) -- Filtre cruise
	AND d.IdPassenger <> 0					 -- Document bien rataché
	AND (CONVERT(varchar, d.ReceiptDate, 112) >= CONVERT(varchar, @startDate, 112) OR @startDate is null)	-- Filtre date
	AND (CONVERT(varchar, d.ReceiptDate, 112) <= CONVERT(varchar, @endDate, 112) OR @endDate is null)		-- Filtre date
RETURN @Nb
END
GO




/****** Object:  StoredProcedure [dbo].[QmStatistics]    Script Date: 18/01/2019 10:31:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
	dbo.QmStatisticsAdvice(@startDate, @endDate, c.IdShip, c.Id, @waitingList) 'QmWaiting'
	FROM dbo.Cruise as c
	INNER JOIN dbo.Lov l ON l.Id = c.IdShip

END
GO

GRANT EXECUTE ON QmStatistics TO User_Ponant
GO	

GRANT EXECUTE ON GetCruises TO User_Ponant
GO

GRANT EXECUTE ON GetShips TO User_Ponant
GO