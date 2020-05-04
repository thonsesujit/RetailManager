CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
	set nocount on;
	SELECT Id, ProductName, [Description], RetailPrice, QuantityinStock , IsTaxable
	FROM dbo.Product
	ORDER BY ProductName

RETURN 0
