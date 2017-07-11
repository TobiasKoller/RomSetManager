namespace RomSetManager.Services
{
    public class ServiceProvider : IServiceProvider
    {
        public ServiceProvider()
        {
            ConfigurationService = new ConfigurationService();
        }

        public ConfigurationService ConfigurationService { get; set; }
    }
}
