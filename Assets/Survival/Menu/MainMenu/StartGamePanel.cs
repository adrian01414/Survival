using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class StartGamePanel : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [Space]
    [SerializeField] private TMP_Dropdown _lobbyTypeDropdown;
    [SerializeField] private Toggle _multiplayerToggle;
    [SerializeField] private CanvasGroup _lobbyTypeCanvasGroup;
    [SerializeField] private CanvasGroup _playerCountCanvasGroup;
    [SerializeField] private TMP_InputField _playerCountInputField;
    [SerializeField] private TMP_Text _playerMaxCountText;

    private Dictionary<string, ELobbyType> _lobbyType = new();

    private LobbyInfo _lobbyInfo;

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _multiplayerToggle.onValueChanged.AddListener(MultiplayerToggleChange);
        _playerCountInputField.onSubmit.AddListener(ChangePlayerCount);
        _playerCountInputField.onDeselect.AddListener(ChangePlayerCount);
    }

    [Inject]
    public void Construct(LobbyInfo lobbyInfo)
    {
        _lobbyInfo = lobbyInfo;
    }

    private void Awake()
    {
        _multiplayerToggle.isOn = false;
        MultiplayerToggleChange(_multiplayerToggle.isOn);

        _lobbyTypeDropdown.ClearOptions();
        _lobbyTypeDropdown.AddOptions(new List<string> {"Friends only", "Public" });

        _lobbyType.Add("Friends only", ELobbyType.k_ELobbyTypeFriendsOnly);
        _lobbyType.Add("Public", ELobbyType.k_ELobbyTypePublic);

        _playerCountInputField.text = _lobbyInfo.MaxConnections.ToString();
        _playerMaxCountText.text = $"Max: {_lobbyInfo.MaxPlayerCount}";
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

        SceneManager.LoadScene("WorldScene");
    }

    private void MultiplayerToggleChange(bool value)
    {
        if (value)
        {
            _lobbyTypeCanvasGroup.alpha = 1;
            _lobbyTypeCanvasGroup.interactable = true;

            _playerCountCanvasGroup.alpha = 1;
            _playerCountInputField.interactable = true;
        } else
        {
            _lobbyTypeCanvasGroup.alpha = 0.3f;
            _lobbyTypeCanvasGroup.interactable = false;

            _playerCountCanvasGroup.alpha = 0.3f;
            _playerCountInputField.interactable = false;
        }
    }

    private void ChangePlayerCount(string str)
    {
        int value = 0;
        try
        {
            value = int.Parse(str);
        } catch 
        {
            value = 2;
        }

        if (value < 2)
        {
            _lobbyInfo.MaxConnections = 2;
        } else if(value > _lobbyInfo.MaxPlayerCount)
        {
            _lobbyInfo.MaxConnections = _lobbyInfo.MaxPlayerCount;
        } else
        {
            _lobbyInfo.MaxConnections = value;
        }
        _playerCountInputField.text = _lobbyInfo.MaxConnections.ToString();
    }

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(StartGame);
        _multiplayerToggle.onValueChanged.RemoveListener(MultiplayerToggleChange);
        _playerCountInputField.onSubmit.RemoveListener(ChangePlayerCount);
        _playerCountInputField.onDeselect.RemoveListener(ChangePlayerCount);
    }
}
