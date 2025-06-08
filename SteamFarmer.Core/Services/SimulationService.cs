using Facepunch.Steamworks;
using SteamFarmer.Core.Services;
using System;
using System.Threading;

namespace SteamFarmer.Core.Services
{
    public class SimulationService : ISimulationService
    {
        private Client _client;
        private Thread _thread;
        private bool _running;

        public void Start(uint appId)
        {
            // инициализируем Facepunch-клиент как “игру” с любым AppID (можно 480)
            _client = new Client(new AppId(105600)); 
            // говорим Steam-клиенту, что мы играем в нужный appId
            _client.Friends.SetPlayedGame(new PlayedGame(appId));

            _running = true;
            _thread = new Thread(() =>
            {
                while (_running)
                {
                    _client.Tick();
                    Thread.Sleep(15_000);
                }
            })
            { IsBackground = true };
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _thread?.Join();
            // сбрасываем статус
            _client.Friends.SetPlayedGame(new PlayedGame(0));
            _client.Dispose();
        }
    }
}
