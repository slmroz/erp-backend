USE slmroz_erpbase
GO
/****** Object:  Table [Auth].[User]    Script Date: 04.02.2026 22:29:56 ******/
SET ANSI_NULLS ON
GO

CREATE SCHEMA [Auth]

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Auth].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](200) NOT NULL,
	[Role] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [Auth].[User] ON 
GO
INSERT [Auth].[User] ([Id], [Email], [Password], [Role], [CreatedAt]) VALUES (1, N'slmroz@wp.pl', N'AQAAAAIAAYagAAAAEApEW1OvTTVoQtpYk0A9deqsLDxNnpcVV0/QstdKoS20yC/tLepdZnxRdCrjpJBQXA==', 2, CAST(N'2025-01-02T15:16:41.3110143' AS DateTime2))
GO
SET IDENTITY_INSERT [Auth].[User] OFF
GO
ALTER TABLE [Auth].[User] ADD  DEFAULT ((1)) FOR [Role]
GO

CREATE TABLE [Crm].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] nvarchar(200),
	[TaxId] nvarchar(100),
	[Address] nvarchar(100),
	[ZipCode] nvarchar(10),
	[City] nvarchar(50),
	[Country] nvarchar(50),
	[Www] nvarchar(100),
	[Facebook] nvarchar(100),
	RemovedAt datetime ,
	LastUpdatedAt datetime 
)

ALTER TABLE Crm.Customer ADD RemovedAt datetime 

select * from [Auth].[User]

delete from [Crm].[Customer]

select * from [Crm].[Customer]

INSERT INTO [Crm].[Customer] ([Name], [TaxId], [Address], [ZipCode], [City], [Country], [Www], [Facebook], [LastModifiedAt])
VALUES
('Microsoft Corporation', 'US-746501', 'One Microsoft Way', '98052', 'Redmond', 'USA', 'www.microsoft.com', 'facebook.com/microsoft', GETDATE()),
('Apple Inc.', 'US-990122', 'One Apple Park Way', '95014', 'Cupertino', 'USA', 'www.apple.com', 'facebook.com/apple', GETDATE()),
('Google LLC', 'US-102933', '1600 Amphitheatre Parkway', '94043', 'Mountain View', 'USA', 'www.google.com', 'facebook.com/google', GETDATE()),
('Amazon.com Inc.', 'US-445566', '410 Terry Ave N', '98109', 'Seattle', 'USA', 'www.amazon.com', 'facebook.com/amazon', GETDATE()),
('Meta Platforms Inc.', 'US-887711', '1 Hacker Way', '94025', 'Menlo Park', 'USA', 'www.meta.com', 'facebook.com/facebook', GETDATE()),
('Tesla Inc.', 'US-554422', '1 Tesla Road', '78725', 'Austin', 'USA', 'www.tesla.com', 'facebook.com/tesla', GETDATE()),
('Samsung Electronics', 'KR-112233', '129 Samsung-ro', '06735', 'Seoul', 'South Korea', 'www.samsung.com', 'facebook.com/samsung', GETDATE()),
('Toyota Motor Corp', 'JP-998877', '1 Toyota-cho', '471-8571', 'Toyota City', 'Japan', 'www.toyota-global.com', 'facebook.com/toyota', GETDATE()),
('Volkswagen Group', 'DE-443322', 'Berliner Ring 2', '38440', 'Wolfsburg', 'Germany', 'www.volkswagen.com', 'facebook.com/volkswagen', GETDATE()),
('Coca-Cola Co', 'US-221100', '1 Coca-Cola Plaza', '30313', 'Atlanta', 'USA', 'www.coca-cola.com', 'facebook.com/cocacola', GETDATE()),
('PepsiCo Inc', 'US-332211', '700 Anderson Hill Rd', '10577', 'Purchase', 'USA', 'www.pepsico.com', 'facebook.com/pepsico', GETDATE()),
('Intel Corporation', 'US-665544', '2200 Mission College Blvd', '95054', 'Santa Clara', 'USA', 'www.intel.com', 'facebook.com/intel', GETDATE()),
('NVIDIA Corp', 'US-778899', '2788 San Tomas Expressway', '95051', 'Santa Clara', 'USA', 'www.nvidia.com', 'facebook.com/nvidia', GETDATE()),
('Netflix Inc', 'US-110022', '121 Albright Way', '95032', 'Los Gatos', 'USA', 'www.netflix.com', 'facebook.com/netflix', GETDATE()),
('Nike Inc', 'US-889900', 'One Bowerman Drive', '97005', 'Beaverton', 'USA', 'www.nike.com', 'facebook.com/nike', GETDATE()),
('Adidas AG', 'DE-776655', 'Adi-Dassler-Strasse 1', '91074', 'Herzogenaurach', 'Germany', 'www.adidas.com', 'facebook.com/adidas', GETDATE()),
('McDonalds Corp', 'US-556677', '110 N Carpenter St', '60607', 'Chicago', 'USA', 'www.mcdonalds.com', 'facebook.com/mcdonalds', GETDATE()),
('Starbucks Corp', 'US-443355', '2401 Utah Ave S', '98134', 'Seattle', 'USA', 'www.starbucks.com', 'facebook.com/starbucks', GETDATE()),
('Disney (The Walt Disney Co)', 'US-991188', '500 S Buena Vista St', '91521', 'Burbank', 'USA', 'www.disney.com', 'facebook.com/disney', GETDATE()),
('Sony Group Corp', 'JP-334455', '1-7-1 Konan', '108-0075', 'Tokyo', 'Japan', 'www.sony.com', 'facebook.com/sony', GETDATE()),
('BMW AG', 'DE-112244', 'Petuelring 130', '80809', 'Munich', 'Germany', 'www.bmw.com', 'facebook.com/bmw', GETDATE()),
('Mercedes-Benz Group', 'DE-556644', 'Mercedesstrasse 120', '70372', 'Stuttgart', 'Germany', 'www.mercedes-benz.com', 'facebook.com/mercedesbenz', GETDATE()),
('Ford Motor Co', 'US-667788', '1 American Rd', '48126', 'Dearborn', 'USA', 'www.ford.com', 'facebook.com/ford', GETDATE()),
('IBM Corp', 'US-223344', '1 New Orchard Rd', '10504', 'Armonk', 'USA', 'www.ibm.com', 'facebook.com/ibm', GETDATE()),
('Oracle Corp', 'US-334422', '2300 Oracle Way', '78741', 'Austin', 'USA', 'www.oracle.com', 'facebook.com/oracle', GETDATE()),
('Cisco Systems Inc', 'US-445511', '170 West Tasman Dr', '95134', 'San Jose', 'USA', 'www.cisco.com', 'facebook.com/cisco', GETDATE()),
('Salesforce Inc', 'US-551122', '415 Mission St', '94105', 'San Francisco', 'USA', 'www.salesforce.com', 'facebook.com/salesforce', GETDATE()),
('Adobe Inc', 'US-662233', '345 Park Avenue', '95110', 'San Jose', 'USA', 'www.adobe.com', 'facebook.com/adobe', GETDATE()),
('Walmart Inc', 'US-773344', '702 SW 8th St', '72716', 'Bentonville', 'USA', 'www.walmart.com', 'facebook.com/walmart', GETDATE()),
('Visa Inc', 'US-884455', 'PO Box 8999', '94128', 'San Francisco', 'USA', 'www.visa.com', 'facebook.com/visa', GETDATE()),
('Mastercard Inc', 'US-995566', '2000 Purchase St', '10577', 'Purchase', 'USA', 'www.mastercard.com', 'facebook.com/mastercard', GETDATE()),
('JPMorgan Chase & Co', 'US-116677', '383 Madison Ave', '10179', 'New York', 'USA', 'www.jpmorganchase.com', 'facebook.com/jpmorgan', GETDATE()),
('Goldman Sachs Group', 'US-227788', '200 West St', '10282', 'New York', 'USA', 'www.goldmansachs.com', 'facebook.com/goldmansachs', GETDATE()),
('American Express', 'US-338899', '200 Vesey St', '10285', 'New York', 'USA', 'www.americanexpress.com', 'facebook.com/americanexpress', GETDATE()),
('PayPal Holdings', 'US-449900', '2211 North First St', '95131', 'San Jose', 'USA', 'www.paypal.com', 'facebook.com/paypal', GETDATE()),
('Uber Technologies', 'US-550011', '1515 3rd St', '94158', 'San Francisco', 'USA', 'www.uber.com', 'facebook.com/uber', GETDATE()),
('Airbnb Inc', 'US-661122', '888 Brannan St', '94103', 'San Francisco', 'USA', 'www.airbnb.com', 'facebook.com/airbnb', GETDATE()),
('Spotify Technology', 'LU-772233', '42-44 Avenue de la Gare', '1610', 'Luxembourg', 'Luxembourg', 'www.spotify.com', 'facebook.com/spotify', GETDATE()),
('Twitter (X Corp)', 'US-883344', '1355 Market St', '94103', 'San Francisco', 'USA', 'www.x.com', 'facebook.com/x', GETDATE()),
('Zoom Video Comm', 'US-994455', '55 Almaden Blvd', '95113', 'San Jose', 'USA', 'www.zoom.us', 'facebook.com/zoom', GETDATE());