using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : IInitializable, IDisposable, ITickable
{
    public static InputManager Instance { get; private set; } // bad practice, need fabric

    // GameplayDefault
    public InputAction GameplayDefault_BuildMenu { get; private set; }
    public InputAction GameplayDefault_Jump { get; private set; }
    public InputAction GameplayDefault_Sprint { get; private set; }

    // Build Menu
    public InputAction BuildMenu_CloseMenu { get; private set; }

    // Build
    public InputAction BuildPlaceStructure { get; private set; }
    public InputAction BuildStructureRotation { get; private set; }
    public InputAction Build_BuildModeDisable { get; private set; }
    public InputAction Build_BuildMenu { get; private set; }

    public Vector2 MoveAxis;

    private Input _input;
    private GameManager _gameManager;

    private List<InputActionMap> _inputActionMaps = new();

    public InputManager(GameManager gameManager)
    {
        Instance = this;

        _gameManager = gameManager;

        _input = new Input();
        _input.Enable();

        // GameplayDefault
        GameplayDefault_BuildMenu = _input.GameplayDefault.BuildMenu;
        GameplayDefault_Jump = _input.GameplayDefault.Jump;
        GameplayDefault_Sprint = _input.GameplayDefault.Sprint;
        _inputActionMaps.Add(_input.GameplayDefault.Get());
        _input.GameplayDefault.Disable();

        // Build Menu
        BuildMenu_CloseMenu = _input.BuildMenu.CloseMenu;
        _inputActionMaps.Add(_input.BuildMenu.Get());
        _input.BuildMenu.Disable();

        // Build
        BuildPlaceStructure = _input.Build.PlaceStructure;
        BuildStructureRotation = _input.Build.StructureRotation;
        Build_BuildModeDisable = _input.Build.BuildModeDisable;
        Build_BuildMenu = _input.Build.BuildMenu;
        _inputActionMaps.Add(_input.Build.Get());
        _input.Build.Disable();
    }

    public void Initialize()
    {
        _gameManager.OnGameStateChanged += ChangeInputState;

        GameplayDefault_BuildMenu.performed += _gameManager.SetBuildMenuState;

        BuildMenu_CloseMenu.performed += _gameManager.SetGameplayDefaultState;

        Build_BuildModeDisable.performed += _gameManager.SetGameplayDefaultState;
        Build_BuildMenu.performed += _gameManager.SetBuildMenuState;
    }

    public void Tick()
    {
        MoveAxis = _input.GameplayDefault.enabled ?
            _input.GameplayDefault.Move.ReadValue<Vector2>() :
            _input.Build.Move.enabled ? _input.Build.Move.ReadValue<Vector2>() :
            Vector2.zero;
    }

    private void ChangeInputState(GameState state)
    {
        switch (state)
        {
            case GameState.Build:
                DisableAllActionMaps();
                _input.Build.Enable();
                break;
            case GameState.GameplayDefault:
                DisableAllActionMaps();
                _input.GameplayDefault.Enable();
                break;
            case GameState.BuildMenu:
                DisableAllActionMaps();
                _input.BuildMenu.Enable();
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
        GameplayDefault_BuildMenu.performed -= _gameManager.SetBuildState;
    }
}
