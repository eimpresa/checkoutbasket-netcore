namespace CheckoutBasket.Configuration
{
    public interface IApplicationSettings
    {
        string TokenIssuer { get; }
        string TokenAudience { get; }
        byte[] TokenSigningKey { get; }
    }
}