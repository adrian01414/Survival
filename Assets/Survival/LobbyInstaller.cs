using Steamworks;
using UnityEngine;
using Zenject;

public class LobbyInstaller : MonoInstaller
{
    public override void Start()
    {
        base.Start();

        ELobbyType lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        int maxConnections = 1;

        SteamMatchmaking.CreateLobby(lobbyType, maxConnections);
    }

    public override void InstallBindings()
    {
    }
}
