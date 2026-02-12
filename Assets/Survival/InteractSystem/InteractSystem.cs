using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InteractSystem : MonoBehaviour
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
            }
        }
    }

    private void PerformInteract(InputAction.CallbackContext context)
    {

        if (!_currentInteractInfo) return;

        if(_currentInteractInfo.InteractableObject is Door)
        {
            (_currentInteractInfo.InteractableObject as Door).playerPosition = _camera.transform.position;
        }

        (_currentInteractInfo.InteractableObject as IInteractable).Interact();
    }

    private void OnDisable()
    {
        _inputManager.GameplayDefault_Interact.performed -= PerformInteract;
    }
}
