using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuildMenu : MonoBehaviour
{
    public GameObject ScrollView;

    public List<BuildMenuItem> Items;

    private GameStateManager _gameManager;
    private BuildSystem _buildSystem;

    [Inject]
    public void Construct(GameStateManager gameManager, BuildSystem buildSystem)
    {
        _gameManager = gameManager;
        _buildSystem = buildSystem;
    }

    private void OnEnable()
    {
        _gameManager.OnGameStateChanged += ChangeMenuVisible;
        foreach (var item in Items)
        {
            item.OnStructureInfoChose += SetStructureInfo;
        }
    }

    private void Awake()
    {
        ScrollView.SetActive(false);
    }

    private void ChangeMenuVisible(GameState state)
    {
        if(state == GameState.BuildMenu)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    public void Show()
    {
        ScrollView.SetActive(true);
    }

    public void Hide()
    {
        ScrollView.SetActive(false);
    }

    private void SetStructureInfo(StructureInfo structureInfo)
    {
        _buildSystem.CurrentStructureInfo = structureInfo;
        _gameManager.State = GameState.Build;
    }

    private void OnDisable()
    {
        _gameManager.OnGameStateChanged -= ChangeMenuVisible;
        foreach (var item in Items)
        {
            item.OnStructureInfoChose -= SetStructureInfo;
        }
    }
}
