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
            .BindInterfacesAndSelfTo<InputManager>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<GameManager>()
            .AsSingle()
            .NonLazy();

        Container
            .BindInterfacesAndSelfTo<CursorManager>()
            .AsSingle()
            .NonLazy();
    }
}
