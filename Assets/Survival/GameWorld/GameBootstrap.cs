using Mirror;
using Steamworks;
using UnityEngine;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    public NetworkManager NetworkManagerPrefab;

    private LobbyInfo _lobbyInfo;

    [Inject]
    public void Construct(LobbyInfo lobbyInfo)
    {
        _lobbyInfo = lobbyInfo;
    }

    private void Awake()
    {
        if (!NetworkManager.singleton)
        {
            Instantiate(NetworkManagerPrefab);
        }

        if(NetworkManagerPrefab is SteamNetworkManager)
        {
            SteamMatchmaking.CreateLobby(_lobbyInfo.LobbyType, _lobbyInfo.MaxConnections);
        }
    }
}
