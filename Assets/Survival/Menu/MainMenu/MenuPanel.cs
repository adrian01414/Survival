using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [Header("Start Game")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private StartGamePanel _startGamePanel;

    private List<GameObject> _panels = new();
    private GameObject _currentActivePanel;

    private void Awake()
    {
        _panels.Add(_startGamePanel.gameObject);

        foreach (GameObject panel in _panels)
        {
            panel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(ShowStartGamePanel);
    }

    private void ShowStartGamePanel()
    {
        ShowPanel(_startGamePanel.gameObject);
    }

    private void ShowPanel(GameObject panel)
    {
        if (!_currentActivePanel)
        {
            panel.gameObject.SetActive(true);
            _currentActivePanel = panel.gameObject;
        } else
        {
            _currentActivePanel.SetActive(false);
            panel.gameObject.SetActive(true);
            _currentActivePanel = null;
        }
    }

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(ShowStartGamePanel);
    }
}
