using Mirror;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class PlayerName : NetworkBehaviour
{
    private TextMesh _text;

    [SyncVar(hook = nameof(OnNameChanged))] private string playerName;

    private void Awake() => _text = GetComponent<TextMesh>();

    public override void OnStartClient()
    {
        if (isOwned)
        {
            CmdSetPlayerName(SteamFriends.GetPersonaName());
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate() => transform.forward = Camera.main.transform.forward;

    [Command]
    private void CmdSetPlayerName(string name) => playerName = name;

    private void OnNameChanged(string oldName, string newName) => _text.text = newName;
}
