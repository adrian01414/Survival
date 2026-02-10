using System;
using UnityEngine;
using Zenject;

public class CursorManager : IInitializable, IDisposable
{
    private GameManager _gameManager;

    public CursorManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        ConfinedCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ConfinedCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Initialize()
    {
        _gameManager.OnGameStateChanged += ChangeCursorState;
    }

    private void ChangeCursorState(GameState gameState)
    {
        if(gameState == GameState.GameplayDefault || gameState == GameState.Build)
        {
            LockCursor();
        } else
        {
            ConfinedCursor();
        }
    }

    public void Dispose()
    {
        _gameManager.OnGameStateChanged -= ChangeCursorState;
    }
}
