CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS
begin
	set nocount on;
	SELECT Id, ProductName, [Description], RetailPrice, QuantityinStock , IsTaxable
	FROM dbo.Product
	where Id = @Id;
end