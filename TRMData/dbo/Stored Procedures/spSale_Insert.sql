CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money

AS
begin
	set nocount on;

	insert into dbo.Sale(CashierId, SaleDate, SubTotal, Tax, Total)
	values (@CashierId, @SaleDate, @SubTotal, @Tax, @Total)

	select @Id = SCOPE_IDENTITY();

	--identiy grabs the last identitiy, thats created in this transation. 
	--Scope_Identity : it puts it into our ID value . gets the identy for the given scope. 

	--For Transations using SQL. inside sql server . lots of overhead needs to be taken care. 
		--insert into dbo.SaleDetail(SaleId, ProductId, Quantity, PurchasePrice, Tax)
	 --   select  @Id, ProductId, Quantity, PurchasePrice, Tax
		--from @TVP

end
