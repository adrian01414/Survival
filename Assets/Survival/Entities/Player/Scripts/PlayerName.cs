using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TextMesh _text;

    [SyncVar(hook = nameof(OnNameChanged))] private string playerName;

    public override void OnStartClient()
    {
        if (isOwned)
        {
            if(NetworkManager.singleton is SteamNetworkManager)
            {
                if (SteamManager.Initialized)
                {
                    CmdSetPlayerName(SteamFriends.GetPersonaName());
                }
            }
            _text.gameObject.SetActive(false);
        }
    }

    private void LateUpdate() => _text.transform.forward = Camera.main.transform.forward;

    [Command]
    private void CmdSetPlayerName(string name) => playerName = name;

    private void OnNameChanged(string oldName, string newName) => _text.text = newName;
}
