using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FoodDelivery.Storage
{
    public class AuthOptions
    {
        public const string ISSUER = "FoodDeliveryServer";
        public const string AUDIENCE = "FoodDeliveryClient";
        public static class AccessToken
        {
            const string KEY = "access_token_key_fooddelivery123";           
            public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            public const int LifeTime = 5;
        }

        public static class RefreshToken
        {
            const string KEY = "fooddelivery12_refresh_token_key";
            public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            public const int LifeTime = 30;
        }
    }
}
