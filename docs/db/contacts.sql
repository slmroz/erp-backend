-- COMPLETE INSERT: 2 kontakty dla każdego z 38 klientów (Id 3-42) = 76 rekordów
INSERT INTO Crm.Contact (CustomerId, FirstName, LastName, PhoneNo, Email) VALUES
-- 3. Microsoft Corporation
(3, N'Satya', N'Nadella', '+1-425-703-8000', 'satya.nadella@microsoft.com'),
(3, N'Amy', N'Hood', '+1-425-706-4400', 'amy.hood@microsoft.com'),

-- 4. Apple Inc.
(4, N'Tim', N'Cook', '+1-408-996-1010', 'tim.cook@apple.com'),
(4, N'Luca', N'Maestri', '+1-408-974-2005', 'luca.maestri@apple.com'),

-- 5. Google LLC
(5, N'Sundar', N'Pichai', '+1-650-253-0000', 'sundar@google.com'),
(5, N'Ruth', N'Porat', '+1-650-253-0001', 'ruth.porat@google.com'),

-- 6. Amazon.com Inc.
(6, N'Andy', N'Jassy', '+1-206-266-1000', 'andy.jassy@amazon.com'),
(6, N'Brian', N'Olsavsky', '+1-206-266-2187', 'brian.olsavsky@amazon.com'),

-- 7. Meta Platforms Inc.
(7, N'Mark', N'Zuckerberg', '+1-650-543-4800', 'mark@fb.com'),
(7, N'Sheryl', N'Sandberg', '+1-650-543-4801', 'sheryl@fb.com'),

-- 8. Tesla Inc.
(8, N'Elon', N'Musk', '+1-512-516-8177', 'elon.musk@tesla.com'),
(8, N'Zach', N'Kirkhorn', '+1-512-516-4800', 'zach.kirkhorn@tesla.com'),

-- 9. Samsung Electronics
(9, N'Jun', N'Suh', '+82-2-2255-0114', 'jun.suh@samsung.com'),
(9, N'Kinam', N'Kim', '+82-2-2255-8221', 'kinam.kim@samsung.com'),

-- 10. Toyota Motor Corp
(10, N'Akio', N'Toyoda', '+81-565-28-2121', 'akio.toyoda@toyota.co.jp'),
(10, N'Kenta', N'Kon', '+81-565-28-2122', 'kenta.kon@toyota.co.jp'),

-- 11. Volkswagen Group
(11, N'Herbert', N'Diess', '+49-5361-9-1', 'herbert.diess@volkswagen.de'),
(11, N'Arno', N'Antlitz', '+49-5361-9-41111', 'arno.antlitz@volkswagen.de'),

-- 12. Coca-Cola Co
(12, N'James', N'Quincey', '+1-404-676-2121', 'james.quincey@coca-cola.com'),
(12, N'John', N'Murphy', '+1-404-676-2683', 'john.murphy@coca-cola.com'),

-- 13. PepsiCo Inc
(13, N'Ramon', N'Laguarta', '+1-914-253-2000', 'ramon.laguarta@pepsico.com'),
(13, N'Hugh', N'Johnston', '+1-914-253-3055', 'hugh.johnston@pepsico.com'),

-- 14. Intel Corporation
(14, N'Pat', N'Gelsinger', '+1-408-765-8080', 'pat.gelsinger@intel.com'),
(14, N'David', N'Zinsner', '+1-408-765-4422', 'david.zinsner@intel.com'),

-- 15. NVIDIA Corp
(15, N'Jensen', N'Huang', '+1-408-486-2000', 'jensen.huang@nvidia.com'),
(15, N'Colette', N'Kress', '+1-408-486-2345', 'colette.kress@nvidia.com'),

-- 16. Netflix Inc
(16, N'Reed', N'Hastings', '+1-408-540-3700', 'reed.hastings@netflix.com'),
(16, N'Ted', N'Sarandos', '+1-408-540-3701', 'ted.sarandos@netflix.com'),

-- 17. Nike Inc
(17, N'John', N'Donahoe', '+1-503-671-6453', 'john.donahoe@nike.com'),
(17, N'Andy', N'Campion', '+1-503-671-6682', 'andy.campion@nike.com'),

-- 18. Adidas AG
(18, N'Kasper', N'Rørsted', '+49-9132-84-0', 'kasper.rorsted@adidas.com'),
(18, N'Harm', N'Ohlmeyer', '+49-9132-84-3888', 'harm.ohlmeyer@adidas.com'),

-- 19. McDonalds Corp
(19, N'Chris', N'Kempczinski', '+1-630-623-3000', 'chris.kempczinski@mcd.com'),
(19, N'Ian', N'Borden', '+1-630-623-5000', 'ian.borden@mcd.com'),

-- 20. Starbucks Corp
(20, N'Laxman', N'Narayan', '+1-206-447-1575', 'laxman.narayan@starbucks.com'),
(20, N'Rachel', N'Argent', '+1-206-318-1575', 'rachel.argent@starbucks.com'),

-- 21. Disney
(21, N'Bob', N'Iger', '+1-818-560-1000', 'bob.iger@disney.com'),
(21, N'Alan', N'Braverman', '+1-818-560-1001', 'alan.braverman@disney.com'),

-- 22. Sony Group Corp
(22, N'Kenichiro', N'Yoshida', '+81-3-6748-2111', 'kenichiro.yoshida@sony.com'),
(22, N'Hiroki', N'Totoki', '+81-3-6748-2112', 'hiroki.totoki@sony.com'),

-- 23. BMW AG
(23, N'Oliver', N'Zipse', '+49-89-382-0', 'oliver.zipse@bmw.de'),
(23, N'Ilse', N'Koerner', '+49-89-382-30130', 'ilse.koerner@bmw.de'),

-- 24. Mercedes-Benz Group
(24, N'Ola', N'Källenius', '+49-711-17-0', 'ola.kallenius@mercedes-benz.com'),
(24, N'Harald', N'Wilhelm', '+49-711-17-47410', 'harald.wilhelm@mercedes-benz.com'),

-- 25. Ford Motor Co
(25, N'Jim', N'Farley', '+1-313-322-3000', 'jim.farley@ford.com'),
(25, N'John', N'Rowley', '+1-313-845-8540', 'john.rowley@ford.com'),

-- 26. IBM Corp
(26, N'Arvind', N'Krishna', '+1-914-499-1900', 'arvind.krishna@ibm.com'),
(26, N'Michelle', N'Holthaus', '+1-914-499-1911', 'michelle.holthaus@ibm.com'),

-- 27. Oracle Corp
(27, N'Safra', N'Catz', '+1-650-506-7000', 'safra.catz@oracle.com'),
(27, N'Ellen', N'Simmons', '+1-650-506-7001', 'ellen.simmons@oracle.com'),

-- 28. Cisco Systems Inc
(28, N'Chuck', N'Robbins', '+1-408-526-4000', 'chuck.robbins@cisco.com'),
(28, N'Wendy', N'East', '+1-408-527-8210', 'wendy.east@cisco.com'),

-- 29. Salesforce Inc
(29, N'Marc', N'Benioff', '+1-415-901-7000', 'marc.benioff@salesforce.com'),
(29, N'Bret', N'Taylor', '+1-415-901-7070', 'bret.taylor@salesforce.com'),

-- 30. Adobe Inc
(30, N'Shulman', N'Vera', N'+1-408-536-6000', 'shulman.vera@adobe.com'),
(30, N'Dan', N'Durrant', '+1-408-536-6010', 'dan.durrant@adobe.com'),

-- 31. Walmart Inc
(31, N'Doug', N'McMillon', '+1-479-273-4000', 'doug.mcmillon@walmart.com'),
(31, N'Machelle', N'Sanders', '+1-479-273-4010', 'machelle.sanders@walmart.com'),

-- 32. Visa Inc
(32, N'Al Kelly', N'', '+1-650-432-3200', 'alkelly@visa.com'),
(32, N'Chris', N'Newman', '+1-650-432-3210', 'chris.newman@visa.com'),

-- 33. Mastercard Inc
(33, N'Michael', N'Miebach', '+1-914-249-2000', 'michael.miebach@mastercard.com'),
(33, N'Linda', N' pistecchia', '+1-914-249-2010', 'linda.pistecchia@mastercard.com'),

-- 34. JPMorgan Chase & Co
(34, N'Jamie', N'Dimon', '+1-212-270-6000', 'jamie.dimon@jpmchase.com'),
(34, N'Jennifer', N'Peirce', '+1-212-270-6010', 'jennifer.peirce@jpmchase.com'),

-- 35. Goldman Sachs Group
(35, N'David', N'Solomon', '+1-212-902-1000', 'david.solomon@gs.com'),
(35, N'Denise', N'Rasmussen', '+1-212-902-1010', 'denise.rasmussen@gs.com'),

-- 36. American Express
(36, N'Stephen', N'Squeri', '+1-212-640-2000', 'stephen.squeri@americanexpress.com'),
(36, N'Jessica', N'Larr', '+1-212-640-2010', 'jessica.larr@americanexpress.com'),

-- 37. PayPal Holdings
(37, N'Dan', N'Schulman', '+1-408-376-7400', 'dan.schulman@paypal.com'),
(37, N'John', N'Raff', '+1-408-376-7410', 'john.raff@paypal.com'),

-- 38. Uber Technologies
(38, N'Dara', N'Khosrowshahi', '+1-415-612-8582', 'dara@uber.com'),
(38, N'Nela', N'Lacasse', '+1-415-612-8590', 'nela.lacasse@uber.com'),

-- 39. Airbnb Inc
(39, N'Brian', N'Chesky', '+1-415-800-1400', 'brian@airbnb.com'),
(39, N'Nathan', N'Blecharczyk', '+1-415-800-1401', 'nathan@airbnb.com'),

-- 40. Spotify Technology
(40, N'Daniel', N'Ek', '+352-26-18-72-1', 'daniel.ek@spotify.com'),
(40, N'Paul', N'Stride', '+352-26-18-72-2', 'paul.stride@spotify.com'),

-- 41. Twitter (X Corp)
(41, N'Elon', N'Musk', '+1-415-222-9670', 'elon@x.com'),
(41, N'Linda', N'Yaccarino', '+1-415-222-9671', 'linda.yaccarino@x.com'),

-- 42. Zoom Video Comm
(42, N'Eric', N'Yuan', '+1-408-453-1500', 'eric.yuan@zoom.us'),
(42, N'Aprille', N'Wensel', '+1-408-453-1501', 'aprille.wensel@zoom.us');
