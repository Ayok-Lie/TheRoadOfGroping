namespace RoadOfGroping.Common.Options
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public static readonly double DefaultAccessTokenExpiresMinutes = 30;
        public static readonly double DefaultRefreshTokenExpiresDays = 50;

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string SecretKey { get; set; }

        public double AccessTokenExpiresMinutes { get; set; } = DefaultAccessTokenExpiresMinutes;

        public double RefreshTokenExpiresDays { get; set; } = DefaultRefreshTokenExpiresDays;
    }
}