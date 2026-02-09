using System;
using UnityEngine.InputSystem;
using Zenject;

public class GameManager
{
    public event Action<GameState> OnGameStateChanged;

    private GameState _state;
    public GameState State
    {
        get { return _state; }
        set
        {
            _state = value;
            OnGameStateChanged?.Invoke(value);
        }
    }

    public void SetPlayerBuildState(InputAction.CallbackContext callback) => State = GameState.PlayerBuild;

    public void SetGameplayDefaultState(InputAction.CallbackContext callback) => State = GameState.GameplayDefault;
}

public enum GameState
{
    Menu,
    Loading,
    GameplayDefault,
    PlayerBuild
}