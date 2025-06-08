namespace SteamFarmer.Core.Services
{
    public interface ISimulationService
    {
        void Start(uint appId);
        void Stop();
    }
}
