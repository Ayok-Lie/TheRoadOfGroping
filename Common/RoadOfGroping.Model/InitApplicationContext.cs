namespace RoadOfGroping.Model
{
    public class InitApplicationContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public InitApplicationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}