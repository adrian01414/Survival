using Mirror;
using Steamworks;
using System.ComponentModel;
using UnityEngine;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    public NetworkManager NetworkManagerPrefab;

    private LobbyInfo _lobbyInfo;
    private GameManager _gameManager;
    private NetworkManager _networkManager;
    private ResourceBank _resourceBank;

    [Inject]
    public void Construct(LobbyInfo lobbyInfo, GameManager gameManager, ResourceBank resourceBank)
    {
        _lobbyInfo = lobbyInfo;
        _gameManager = gameManager;
        _resourceBank = resourceBank;
    }

    private void Awake()
    {
        if (!NetworkManager.singleton && NetworkManagerPrefab)
        {
            _networkManager = Instantiate(NetworkManagerPrefab);

            if (_networkManager is SteamNetworkManager)
            {
                SteamMatchmaking.CreateLobby(_lobbyInfo.LobbyType, _lobbyInfo.MaxConnections);
            }
        }
    }

    private void Start()
    {
        _gameManager.State = GameState.GameplayDefault;

        _resourceBank.SetResource<WoodResource>(100);
    }
}
