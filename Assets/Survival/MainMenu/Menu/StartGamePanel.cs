using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [Space]
    [SerializeField] private TMP_Dropdown _lobbyTypeDropdown;
    [SerializeField] private Toggle _multiplayerToggle;
    [SerializeField] private CanvasGroup _lobbyTypeCanvasGroup;
    [Space]
    [SerializeField] private SteamLobby _steamLobby;

    private Dictionary<string, ELobbyType> _lobbyType = new();

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _multiplayerToggle.onValueChanged.AddListener(MultiplayerToggleChange);
    }

    private void Awake()
    {
        _multiplayerToggle.isOn = false;
        MultiplayerToggleChange(_multiplayerToggle.isOn);

        _lobbyTypeDropdown.ClearOptions();
        _lobbyTypeDropdown.AddOptions(new List<string> {"Friends only", "Public" });

        _lobbyType.Add("Friends only", ELobbyType.k_ELobbyTypeFriendsOnly);
        _lobbyType.Add("Public", ELobbyType.k_ELobbyTypePublic);
    }

    private void StartGame()
    {
        ELobbyType lobbyType;
        if (!_multiplayerToggle.isOn)
        {
            lobbyType = ELobbyType.k_ELobbyTypeInvisible;
        }
        else
        {
            if (!_lobbyType.TryGetValue(_lobbyTypeDropdown.captionText.text, out lobbyType))
            {
                lobbyType = ELobbyType.k_ELobbyTypeInvisible;
            }
        }

        SceneManager.LoadScene("ControllerTestScene"); // 
        int maxConnections = 4;

        SteamMatchmaking.CreateLobby(lobbyType, maxConnections);
    }

    private void MultiplayerToggleChange(bool value)
    {
        if (value)
        {
            _lobbyTypeCanvasGroup.alpha = 1;
            _lobbyTypeCanvasGroup.interactable = true;
        } else
        {
            _lobbyTypeCanvasGroup.alpha = 0.3f;
            _lobbyTypeCanvasGroup.interactable = false;
        }
    }

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(StartGame);
    }
}
