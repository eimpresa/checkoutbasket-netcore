namespace CheckoutBasket.Repositories
{
    public interface ICheckoutBasketRepositories
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
    }
}