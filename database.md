
### Criação do banco de dados

```sql
CREATE DATABASE interplayers;
GO

USE interplayers;
GO
```

### Criação da tabela de Produtos e adição de regra para impedir pre�os invalidos

```sql
CREATE TABLE Products (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);
GO

ALTER TABLE Products
ADD CONSTRAINT CK_Product_Price_Positive CHECK (Price > 0);
GO
```

### Criação da tabela de Pedidos

```sql
CREATE TABLE [dbo].[Orders] (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO
```

### Crição da tabela de Itens

```sql
CREATE TABLE [dbo].[OrderItems] (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    SubTotal AS (Quantity * UnitPrice) PERSISTED,
    
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderId)
        REFERENCES [dbo].[Orders](Id)
);
GO
```

### Criação do trigger para atualização do valor total do pedido

```sql
CREATE TRIGGER TR_OrderItems_UpdateTotal
ON [dbo].[OrderItems]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    WITH AffectedOrders AS (
        SELECT OrderId FROM inserted
        UNION
        SELECT OrderId FROM deleted
    )
    UPDATE o
    SET o.Total = (
        SELECT SUM(SubTotal)
        FROM OrderItems oi
        WHERE oi.OrderId = o.Id
    )
    FROM [dbo].[Orders] o
    INNER JOIN AffectedOrders a ON o.Id = a.OrderId;
END;
GO
```

### Criação de store procedure, permitindo consulta de pedidos com filtros
```sql
CREATE PROCEDURE sp_GetOrders
    @OrderId INT = NULL,
    @ProductId INT = NULL,
    @ProductName NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        o.Id,
        o.Total,

        oi.Id,
        oi.ProductId,
        oi.Quantity,
        oi.UnitPrice,
        oi.SubTotal,

        p.Id,
        p.Name,
        p.Price
    FROM Orders o
    LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
    LEFT JOIN Products p ON p.Id = oi.ProductId
    WHERE 
        (@OrderId IS NULL OR o.Id = @OrderId)
        AND (@ProductId IS NULL OR p.Id = @ProductId)
        AND (@ProductName IS NULL OR p.Name LIKE '%' + @ProductName + '%')
    ORDER BY o.Id, oi.Id;
END
GO
```