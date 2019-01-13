using System;
using Microsoft.Extensions.Configuration;

namespace CheckoutBasket.Configuration
{
    public sealed class ApplicationSettings : IApplicationSettings
    {
        private readonly IConfiguration configuration;

        private ApplicationSettings(){}

        public string TokenIssuer { get; private set; }
        public string TokenAudience { get; private set; }
        public byte[] TokenSigningKey { get; private set; }

        public static IApplicationSettings Build(IConfiguration configuration)
        {
            string tokenIssuer = configuration["TokenIssuer"] ?? throw new ArgumentException("Configuration missing: TokenIssuer", "TokenIssuer");
            string tokenAudience = configuration["TokenAudience"] ?? throw new ArgumentException("Configuration missing: TokenAudience", "TokenAudience");
            string signingKey = configuration["SigningKey"] ?? throw new ArgumentException("Configuration missing: SigningKey", "SigningKey");
            return new ApplicationSettings
            {
                TokenIssuer = tokenIssuer,
                TokenAudience = tokenAudience,
                TokenSigningKey = System.Text.Encoding.UTF8.GetBytes(signingKey)
            };
        }
    }
}