using UnityEngine;
using Zenject;

public class ProjectMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<LobbyInfo>()
            .AsSingle();

        Container
            .Bind<GameStateManager>()
            .AsSingle()
            .NonLazy();

        Container
            .BindInterfacesAndSelfTo<InputManager>()
            .AsSingle()
            .NonLazy();

        Container
            .BindInterfacesAndSelfTo<CursorManager>()
            .AsSingle()
            .NonLazy();
    }
}
