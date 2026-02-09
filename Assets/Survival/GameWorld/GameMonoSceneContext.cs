using UnityEngine;
using Zenject;

public class GameMonoSceneContext : MonoInstaller
{
    public BuildSystem BuildSystem;

    public override void InstallBindings()
    {
        Container
            .Bind<BuildSystem>()
            .FromInstance(BuildSystem)
            .AsSingle();
    }
}
