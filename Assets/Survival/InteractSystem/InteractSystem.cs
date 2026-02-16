using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InteractSystem : NetworkBehaviour
{
    public LayerMask InteractableLayer;
    public float InteractDistance = 3f;

    private Camera _camera;

    private InteractInfo _currentInteractInfo;

    //Dependencies
    private InputManager _inputManager;

    [Inject]
    public void Construct(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    private void OnEnable()
    {
        _inputManager.GameplayDefault_Interact.performed += PerformInteract;
    }

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, InteractDistance, InteractableLayer)){
            if(hit.collider.TryGetComponent<InteractInfo>(out var interactInfo))
            {
                _currentInteractInfo = interactInfo;
            } else
            {
                _currentInteractInfo = null;
            }
        } else
        {
            _currentInteractInfo = null;
        }
    }

    private void OpenDoor(uint netId, Vector3 playerPosition)
    {
        var doorGO = NetworkServer.spawned[netId].gameObject;
        var door = doorGO.GetComponent<Door>();
        door.playerPosition = playerPosition;
        door.Interact();
    }

    [Command(requiresAuthority = false)]
    private void CmdOpenDoor(uint netId, Vector3 playerPosition)
    {
        OpenDoor(netId, playerPosition);
    }

    private void PerformInteract(InputAction.CallbackContext context)
    {
        if (!_currentInteractInfo) return;

        var netId = _currentInteractInfo.InteractableObject.GetComponent<NetworkIdentity>().netId;

        if (_currentInteractInfo.InteractableObject is Door)
        {
            var playerPosition = _camera.transform.position;

            if (NetworkServer.active)
            {
                OpenDoor(netId, playerPosition);
            }
            else
            {
                CmdOpenDoor(netId, playerPosition);
            }
        }
    }

    private void OnDisable()
    {
        _inputManager.GameplayDefault_Interact.performed -= PerformInteract;
    }
}
