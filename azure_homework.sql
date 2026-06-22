DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Orders;

CREATE TABLE Categories (
Id INT PRIMARY KEY IDENTITY,
Name NVARCHAR(100) NOT NULL,
Description NVARCHAR(255)
);

CREATE TABLE Products (
Id INT PRIMARY KEY IDENTITY,
Name NVARCHAR(150) NOT NULL,
CategoryId INT NOT NULL REFERENCES Categories(Id),
Price DECIMAL(10, 2) NOT NULL,
Stock INT NOT NULL DEFAULT 0,
CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Orders (
Id INT PRIMARY KEY IDENTITY,
CustomerName NVARCHAR(100) NOT NULL,
OrderDate DATETIME DEFAULT GETDATE(),
Status NVARCHAR(50) DEFAULT 'Pending'
);

CREATE TABLE OrderItems (
Id INT PRIMARY KEY IDENTITY,
OrderId INT NOT NULL REFERENCES Orders(Id),
ProductId INT NOT NULL REFERENCES Products(Id),
Quantity INT NOT NULL,
UnitPrice DECIMAL(10, 2) NOT NULL
);


INSERT INTO Categories (Name, Description) VALUES
('Electronics', 'Phones, Laptops, Tablets'),
('Clothing', 'Mens and Womens Clothing'),
('Books', 'Technical and Fiction'),
('Sports', 'Sports Equipment and Accessories');
INSERT INTO Products (Name, CategoryId, Price, Stock) VALUES

('iPhone 15', 1, 35000.00, 10),
('Samsung Galaxy', 1, 22000.00, 15),
('Laptop Dell', 1, 45000.00, 5),
('T-shirt Nike', 2, 850.00, 50),
('Coat Adidas', 2, 3200.00, 20),
('Clean Code', 3, 650.00, 30),
('The Pragmatic', 3, 580.00, 25),
('Dumbbells 10êã', 4, 1200.00, 12),
('Yoga mat', 4, 450.00, 40),
('Headphones Sony', 1, 4500.00, 8);

INSERT INTO Orders (CustomerName, Status) VALUES
('Ivan Petrenko', 'Completed'),
('Mariya Koval', 'Completed'),
('Oleg Sidorenko', 'Pending'),
('Anya Bondarenko', 'Cancelled'),
('Sergiy Moroz', 'Completed');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 1, 35000.00),
(1, 6, 2, 650.00),
(2, 4, 3, 850.00),
(2, 9, 1, 450.00),
(3, 3, 1, 45000.00),
(3, 8, 2, 1200.00),
(4, 5, 1, 3200.00),
(5, 10,1, 4500.00),
(5, 2, 1, 22000.00),
(5, 7, 2, 580.00);


-- 1
SELECT * 
FROM Products P
WHERE P.CategoryId = 1 AND P.Price > 5000 
ORDER BY P.Price DESC;
-- 2
SELECT P.CategoryId, COUNT(*) as ProductCount, AVG(P.Price) as AveragePrice 
FROM Products P
GROUP BY P.CategoryId
ORDER BY AVG(P.Price) DESC;
-- 3
SELECT O.Id, O.CustomerName, P.Name, OI.Quantity, OI.Quantity * OI.UnitPrice as Total
FROM Orders O
JOIN OrderItems OI ON O.Id = OI.OrderId
JOIN Products P ON P.Id = OI.ProductId
WHERE O.Status = 'Completed';
-- 4
SELECT O.CustomerName, SUM(OI.Quantity * OI.UnitPrice) as TotalSpend
FROM Orders O
JOIN OrderItems OI ON O.Id = OI.OrderId
GROUP BY O.CustomerName
HAVING SUM(OI.Quantity * OI.UnitPrice) > 10000
ORDER BY TotalSpend DESC;
-- 5
SELECT 
	P.Name AS ProductName, 
	C.Name AS CategoryName, 
	P.Price AS ProductPrice
FROM Products P
JOIN Categories C ON C.Id = P.CategoryId
WHERE P.Price > (
	SELECT AVG(P2.Price)
	FROM Products P2
	WHERE P2.CategoryId = P.CategoryId
);
-- 6
WITH cte AS(
	SELECT P.Name, SUM(OI.Quantity) AS TotalQuantity
	FROM Orders O
	JOIN OrderItems OI ON O.Id = OI.OrderId
	JOIN Products P ON P.Id = OI.ProductId
	WHERE O.Status = 'Completed'
	GROUP BY P.Name
)

SELECT TOP 3 * 
FROM cte
ORDER BY TotalQuantity DESC;
-- 7
BEGIN TRANSACTION;
    UPDATE P
    SET Stock = P.Stock - OI.Quantity
    FROM Products P
    JOIN OrderItems OI ON OI.ProductId = P.Id
    WHERE OI.OrderId = 3;

	IF EXISTS (
        SELECT P.Stock
        FROM Products P
        WHERE P.Stock < 0
    )
    BEGIN
        ROLLBACK TRANSACTION;
    END
	ELSE
	BEGIN
	    UPDATE Orders 
		SET Status = 'Completed'
		WHERE Id = 3;
		COMMIT TRANSACTION;
	END

SELECT O.Status
FROM Orders O
WHERE O.Id = 3