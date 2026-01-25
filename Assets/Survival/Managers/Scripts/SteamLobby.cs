using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SteamManager), typeof(SteamNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    private Callback<LobbyCreated_t> LobbyCreated;
    private Callback<GameLobbyJoinRequested_t> JoinRequest;
    private Callback<LobbyEnter_t> LobbyEntered;

    private SteamNetworkManager _networkManager;
    private void Awake()
    {
        instance = this;
        _networkManager = GetComponent<SteamNetworkManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!SteamManager.Initialized) return;

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        print("Lobby created");

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
            LobbyKeys.HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
            LobbyKeys.SceneNameKey,
            SceneManager.GetActiveScene().name);

        _networkManager.maxConnections = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(callback.m_ulSteamIDLobby)) - 1;
        _networkManager.StartHost();
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        print("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        SceneManager.LoadScene(SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), LobbyKeys.SceneNameKey));

        _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), LobbyKeys.HostAddressKey);
        _networkManager.StartClient();
    }
}
