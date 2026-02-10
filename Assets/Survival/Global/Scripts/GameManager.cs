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

    public void SetBuildMenuState(InputAction.CallbackContext callback) => State = GameState.BuildMenu;
    public void SetBuildState(InputAction.CallbackContext callback) => State = GameState.Build;
    public void SetGameplayDefaultState(InputAction.CallbackContext callback) => State = GameState.GameplayDefault;
}

public enum GameState
{
    GameplayDefault,
    Build,
    BuildMenu
}