
-- Récupération du nombre de QM par bateaux et par avis
SELECT Distinct l1.Name AS 'ShipName', l2.Name AS 'Advice', COUNT(p.Id) AS 'NombreQM' FROM Passenger p
LEFT JOIN BookingCruisePassenger bcp ON P.Id = bcp.IdPassenger
LEFT JOIN Booking b ON b.Id = bcp.IdBooking
LEFT JOIN Cruise c ON c.Id = bcp.IdCruise
LEFT JOIN Lov l1 ON l1.Id = c.IdShip
LEFT JOIN Lov l2 ON l2.Id = p.IdAdvice
WHERE l1.Name IS NOT NULL AND l2.Name IS NOT NULL
GROUP BY l1.Name, l2.Name
ORDER BY l1.Name, l2.Name

-- Désactive l'extraction à terre pour les croisières trop anciennes
UPDATE Cruise
SET IsExtract = 0
WHERE IsExtract = 1
AND DATEADD(day, SailingLengthDays, SailingDate) < GETDATE()