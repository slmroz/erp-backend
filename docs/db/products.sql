USE slmroz_erpbase

CREATE TABLE Crm.ProductGroups (
    Id int PRIMARY KEY IDENTITY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(500),
    CreatedAt datetime2 DEFAULT GETDATE()
);

-- INSERT GRUPY (8 kategorii Automotive Parts)
INSERT INTO Crm.ProductGroups (Name, Description) VALUES
(N'Engine Components', N'Silniki i podzespo³y silnikowe'),
(N'Brake Systems', N'Uk³ady hamulcowe i tarcze'),
(N'Suspension & Steering', N'Zawieszenie i uk³ad kierowniczy'),
(N'Transmission', N'Skrzynie biegów i podzespo³y'),
(N'Electrical Systems', N'Elektryka i elektronika'),
(N'Body & Exterior', N'Karoseria i elementy zewnêtrzne'),
(N'Cooling & Heating', N'Uk³ady ch³odzenia i ogrzewania'),
(N'Fasteners & Hardware', N'Sruby, nakrêtki, podk³adki');

CREATE TABLE Crm.Products (
    Id int PRIMARY KEY IDENTITY,
    ProductGroupId int NOT NULL REFERENCES Crm.ProductGroups(Id),
    PartNumber nvarchar(50) NOT NULL,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(500),
    OEMBrand nvarchar(100),
    ListPrice decimal(18,2),
    WeightKg decimal(10,3),
    CreatedAt datetime2 DEFAULT GETDATE()
);


CREATE TABLE Crm.Products (
    Id int PRIMARY KEY IDENTITY,
    ProductGroupId int NOT NULL REFERENCES Crm.ProductGroups(Id),
    PartNumber nvarchar(50) NOT NULL,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(500),
    OEMBrand nvarchar(100),
    ListPrice decimal(18,2),
    WeightKg decimal(10,3),
    CreatedAt datetime2 DEFAULT GETDATE()
);


-- GRUPA 1: ENGINE COMPONENTS (15 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(1, N'TM-ENG-001', N'Piston Set 2.0L Turbo', N'Komplet 4 t³oków + pierœcienie, 82mm', N'Toyota', 1450.00, 2.800),
(1, N'TM-ENG-002', N'Cylinder Head Gasket', N'Uszczelka pod g³owicê aluminiow¹ 16V', N'Toyota', 89.50, 0.450),
(1, N'VW-ENG-003', N'Timing Belt Kit', N'Rozrz¹d kompletny z napinaczem', N'Volkswagen', 245.00, 1.200),
(1, N'BM-ENG-004', N'Oil Pump Assembly', N'Pompa oleju wysokociœnieniowa', N'BMW', 389.00, 1.850),
(1, N'FN-ENG-005', N'Camshaft Position Sensor', N'Czujnik po³o¿enia wa³ka rozrz¹du', N'Ford', 67.90, 0.120);

-- GRUPA 2: BRAKE SYSTEMS (20 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(2, N'TM-BRK-001', N'Front Brake Disc 356mm', N'Tarcza hamulcowa wentylowana 356x30mm', N'Toyota', 189.00, 12.500),
(2, N'TM-BRK-002', N'Brake Pads Semi-Metallic', N'Klocki hamulcowe przednie semi-metaliczne', N'Toyota', 78.50, 1.900),
(2, N'VW-BRK-003', N'Rear Brake Caliper', N'Zacisk hamulcowy tylny prawy', N'Volkswagen', 298.00, 4.200),
(2, N'BM-BRK-004', N'Brake Pad Wear Sensor', N'Czujnik zu¿ycia klocków', N'BMW', 34.90, 0.050),
(2, N'FN-BRK-005', N'Master Cylinder', N'Cylinder g³ówny hamulców', N'Ford', 156.00, 1.650);

-- GRUPA 3: SUSPENSION & STEERING (18 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(3, N'TM-SUS-001', N'Front Control Arm L', N' Wahacz przedni lewy', N'Toyota', 245.00, 5.800),
(3, N'TM-SUS-002', N'Ball Joint Lower', N'Zwrotnica dolna', N'Toyota', 89.00, 0.950),
(3, N'VW-SUS-003', N'Power Steering Pump', N'Pompa wspomagania hydrauliczna', N'Volkswagen', 312.00, 3.200),
(3, N'BM-SUS-004', N'Coil Spring Front', N'Sprê¿yna zawieszenia przednia', N'BMW', 145.00, 4.100),
(3, N'FN-SUS-005', N'Tie Rod End', N'Tuleja koñcówki dr¹¿ka', N'Ford', 56.80, 0.420);

-- GRUPA 4: TRANSMISSION (12 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(4, N'TM-TRN-001', N'Clutch Pressure Plate', N'Tarcza dociskowa sprzêg³a 228mm', N'Toyota', 289.00, 6.500),
(4, N'VW-TRN-002', N'CVT Transmission Fluid', N'Olej przek³adni CVT 4L', N'Volkswagen', 145.00, 3.800),
(4, N'BM-TRN-003', N'Mechatronic Unit DSG', N'Mechatronika DSG 7-biegowa', N'BMW', 2450.00, 8.200),
(4, N'FN-TRN-004', N'Transmission Filter', N'Filtr oleju skrzyni automatycznej', N'Ford', 45.90, 0.280);

-- GRUPA 5: ELECTRICAL SYSTEMS (15 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(5, N'TM-ELC-001', N'Alternator 120A', N'Alternator 14V/120A z regulatorem', N'Toyota', 378.00, 5.600),
(5, N'TM-ELC-002', N'Spark Plugs Iridium', N'Œwiece zap³onowe irydowe x4', N'Toyota', 89.00, 0.200),
(5, N'VW-ELC-003', N'Glow Plug Relay', N'PrzekaŸnik œwiec ¿arowych', N'Volkswagen', 67.50, 0.180),
(5, N'BM-ELC-004', N'Battery Management Sensor', N'Czujnik BMS intel-sensing', N'BMW', 145.00, 0.320);

-- GRUPA 6: BODY & EXTERIOR (10 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(6, N'TM-BDY-001', N'Headlight LED L', N'Reflektor LED prawy xenon+LED', N'Toyota', 1890.00, 7.200),
(6, N'VW-BDY-002', N'Fender Front L', N'B³otnik przedni lewy', N'Volkswagen', 356.00, 8.900),
(6, N'BM-BDY-003', N'Windshield Glass', N'Szyba przednia laminowana', N'BMW', 789.00, 18.500);

-- GRUPA 7: COOLING & HEATING (5 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(7, N'TM-CLD-001', N'Radiator Aluminum', N'Ch³odnica aluminiowa', N'Toyota', 298.00, 6.800),
(7, N'VW-CLD-002', N'Thermostat Housing', N'Obudowa termostatu', N'Volkswagen', 89.50, 1.200);

-- GRUPA 8: FASTENERS & HARDWARE (5 produktów)
INSERT INTO Crm.Products (ProductGroupId, PartNumber, Name, Description, OEMBrand, ListPrice, WeightKg) VALUES
(8, N'TM-FST-001', N'Wheel Bolt M12x1.5', N'Œruba kola M12x1.5x32mm x20', N'Toyota', 45.00, 1.200),
(8, N'VW-FST-002', N'Engine Bolt Kit', N'Komplet œrub silnika M10-M12', N'Volkswagen', 156.00, 2.500);

-- SprawdŸ grupy
SELECT * FROM Crm.ProductGroups;

-- SprawdŸ produkty z grupami
SELECT 
    pg.Name AS GroupName,
    COUNT(p.Id) AS ProductsCount,
    AVG(p.ListPrice) AS AvgPrice
FROM Crm.Products p
JOIN Crm.ProductGroups pg ON p.ProductGroupId = pg.Id
GROUP BY pg.Name;

-- Top 10 najdro¿szych produktów
SELECT TOP 10 p.Name, p.ListPrice, pg.Name AS [Group]
FROM Crm.Products p
JOIN Crm.ProductGroups pg ON p.ProductGroupId = pg.Id
ORDER BY p.ListPrice DESC;
