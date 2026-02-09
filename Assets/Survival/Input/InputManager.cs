using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : IInitializable, IDisposable
{
    // Build
    public InputAction BuildPlaceStructure { get; private set; }
    public InputAction BuildStructureRotation { get; private set; }
    public InputAction Build_BuildModeDisable { get; private set; }

    //GameplayDefault
    public InputAction GameplayDefault_BuildModeEnable { get; private set; }

    private Input _input;
    private GameManager _gameManager;

    private List<InputActionMap> _inputActionMaps = new();

    public InputManager(GameManager gameManager)
    {
        _gameManager = gameManager;

        _input = new Input();
        _input.Enable();

        BuildPlaceStructure = _input.Build.PlaceStructure;
        BuildStructureRotation = _input.Build.StructureRotation;
        Build_BuildModeDisable = _input.Build.BuildModeDisable;
        _inputActionMaps.Add(_input.Build.Get());
        _input.Build.Disable();

        GameplayDefault_BuildModeEnable = _input.GameplayDefault.BuildModeEnable;
        _inputActionMaps.Add(_input.GameplayDefault.Get());
        _input.GameplayDefault.Disable(); //
    }

    public void Initialize()
    {
        _gameManager.OnGameStateChanged += ChangeInputState;

        Build_BuildModeDisable.performed += _gameManager.SetGameplayDefaultState;
        GameplayDefault_BuildModeEnable.performed += _gameManager.SetPlayerBuildState;
    }

    private void ChangeInputState(GameState state)
    {
        switch (state)
        {
            case GameState.PlayerBuild:
                DisableAllActionMaps();
                _input.Build.Enable();
                break;
            case GameState.GameplayDefault:
                DisableAllActionMaps();
                _input.GameplayDefault.Enable();
                break;
        }
    }

    private void DisableAllActionMaps()
    {
        foreach (var map in _inputActionMaps)
        {
            map.Disable();
        }
    }

    public void Dispose()
    {
        _gameManager.OnGameStateChanged -= ChangeInputState;

        Build_BuildModeDisable.performed -= _gameManager.SetGameplayDefaultState;
        GameplayDefault_BuildModeEnable.performed -= _gameManager.SetPlayerBuildState;
    }
}
