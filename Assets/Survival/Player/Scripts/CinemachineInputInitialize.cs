using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineInputInitialize : MonoBehaviour
{
    [SerializeField] private InputActionAsset _actions;
    [SerializeField] private string _defaultMap;
    [SerializeField] private string _lookAction;

    private void Awake()
    {
        var inputActionReference = InputActionReference.Create(_actions.FindActionMap(_defaultMap).FindAction(_lookAction));
        foreach (var controller in GetComponent<CinemachineInputAxisController>().Controllers)
            controller.Input.InputAction = inputActionReference;
    }
}
