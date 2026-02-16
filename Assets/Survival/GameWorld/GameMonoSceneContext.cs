using Mirror;
using Steamworks;
using UnityEngine;
using Zenject;

public class GameMonoSceneContext : MonoInstaller
{
    public NetworkManager NetworkManagerPrefab;

    public BuildSystem BuildSystem;

    public override void InstallBindings()
    {
        Container
            .Bind<BuildSystem>()
            .FromInstance(BuildSystem)
            .AsSingle();

        Container
            .Bind<ResourceBank>()
            .AsSingle();

        Container.BindFactory<TestNetworkManager, TestNetworkManager.Factory>()
            .FromComponentInNewPrefab(NetworkManagerPrefab);
    }
}
