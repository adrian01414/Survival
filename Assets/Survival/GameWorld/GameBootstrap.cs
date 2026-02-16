using Mirror;
using Steamworks;
using UnityEngine;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    public NetworkManager NetworkManagerPrefab;

    private LobbyInfo _lobbyInfo;
    private GameStateManager _gameManager;
    private NetworkManager _networkManager;
    private ResourceBank _resourceBank;

    private TestNetworkManager.Factory _networkManagerFactory;

    [Inject]
    public void Construct(LobbyInfo lobbyInfo, 
        GameStateManager gameManager, 
        ResourceBank resourceBank,
        TestNetworkManager.Factory networkManagerFactory)
    {
        _lobbyInfo = lobbyInfo;
        _gameManager = gameManager;
        _resourceBank = resourceBank;
        _networkManagerFactory = networkManagerFactory;
    }

    private void Awake()
    {
        if (!NetworkManager.singleton && NetworkManagerPrefab)
        {
            _networkManager = _networkManagerFactory.Create();

            if (_networkManager is SteamNetworkManager)
            {
                SteamMatchmaking.CreateLobby(_lobbyInfo.LobbyType, _lobbyInfo.MaxConnections);
            }
        }

        _gameManager.State = GameState.Loading;

        _resourceBank.SetResource<WoodResource>(100);
    }
}
