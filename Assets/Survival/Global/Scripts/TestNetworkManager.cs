using Mirror;
using UnityEngine;
using Zenject;

public class TestNetworkManager : NetworkManager
{
    private GameStateManager _gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public class Factory : PlaceholderFactory<TestNetworkManager> { }
}
