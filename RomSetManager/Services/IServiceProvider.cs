namespace RomSetManager.Services
{
    public interface IServiceProvider
    {
        ConfigurationService ConfigurationService { get; set; }
        BestMatchService BestMatchService { get; set; }
    }
}
