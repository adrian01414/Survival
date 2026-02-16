using System;
using UnityEngine.InputSystem;

public enum GameState
{
    Loading,
    GameplayDefault,
    Build,
    BuildMenu
}

public class GameStateManager
{
    public static GameStateManager Instance { get; private set; }

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

    public GameStateManager()
    {
        Instance = this;
    }

    public void SetBuildMenuState(InputAction.CallbackContext callback) => State = GameState.BuildMenu;
    public void SetBuildState(InputAction.CallbackContext callback) => State = GameState.Build;
    public void SetGameplayDefaultState(InputAction.CallbackContext callback) => State = GameState.GameplayDefault;
}
