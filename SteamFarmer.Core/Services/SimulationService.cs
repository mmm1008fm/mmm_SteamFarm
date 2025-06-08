using Steamworks;
using System;
using System.Threading;

namespace SteamFarmer.Core.Services
{
    public class SimulationService : ISimulationService, IDisposable
    {
        private bool _running;
        private Thread _worker;

        public void Start(uint appId)
        {
            // 1) Инициализация
            if (!SteamAPI.Init())
                throw new InvalidOperationException("Не удалось инициализировать SteamAPI");

            // 2) Устанавливаем “в игре” для нужного AppID
            SteamFriends.SetPlayedGame(new FriendGameInfo_t
            {
                m_gameID = new CGameID(appId),
                m_unGameIP = 0,
                m_unGamePort = 0,
                m_unSpectatorPort = 0,
                m_SteamIDLobby = CSteamID.Nil
            });

            // 3) Фон для колбеков
            _running = true;
            _worker = new Thread(() =>
            {
                while (_running)
                {
                    SteamAPI.RunCallbacks();
                    Thread.Sleep(15_000);
                }
            })
            { IsBackground = true };
            _worker.Start();
        }

        public void Stop()
        {
            _running = false;
            _worker?.Join();
            SteamFriends.ClearPlayedGame(); // сброс статуса
            SteamAPI.Shutdown();
        }

        public void Dispose() => Stop();
    }
}
