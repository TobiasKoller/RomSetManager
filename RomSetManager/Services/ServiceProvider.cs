namespace RomSetManager.Services
{
    public class ServiceProvider : IServiceProvider
    {
        public ServiceProvider()
        {
            ConfigurationService = new ConfigurationService();
            BestMatchService = new BestMatchService();
            
        }

        public ConfigurationService ConfigurationService { get; set; }
        public BestMatchService BestMatchService { get; set; }
    }
}
