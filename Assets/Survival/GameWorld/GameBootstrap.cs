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

    [Inject]
    public void Construct(LobbyInfo lobbyInfo, GameManager gameManager)
    {
        _lobbyInfo = lobbyInfo;
        _gameManager = gameManager;
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
    }
}
