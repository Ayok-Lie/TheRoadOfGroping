namespace RoadOfGroping.Utility.RedisModule
{
    public class RedisCacheOptions
    {
        public bool EnableRedis { get; set; }

        public Redis Redis { get; set; }
    }

    public class Redis
    {
        public string Host { get; set; }

        public string Password { get; set; }

        public int Database { get; set; }

        public bool Ssl { get; set; }
    }
}