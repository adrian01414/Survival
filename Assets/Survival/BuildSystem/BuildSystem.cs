using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BuildSystem : NetworkBehaviour
{
    private static int _structureID = 0;

    public float PreviewDistance = 5f;
    public LayerMask LayerMask;

    public StructureInfo CurrentStructureInfo
    {
        get => _currentStructureInfo;
        set
        {
            if (_currentStructureInfo != value)
            {
                _currentStructureInfo = value;
                ChangePreview();
            }
        }
    }

    private StructureInfo _currentStructureInfo = null;
    private StructurePreview _currentStructurePreview = null;
    private Camera _camera;
    private int _structurePivotLayer;
    private Quaternion _previewRotation = Quaternion.identity;

    private bool _enabled = false;

    private InputManager _inputManager;
    private GameManager _gameManager;

    [Inject]
    public void Construct(InputManager inputManager, GameManager gameManager)
    {
        _inputManager = inputManager;
        _gameManager = gameManager;
    }

    private void OnEnable()
    {
        _inputManager.BuildPlaceStructure.performed += PlaceStructurePerformed;
        _inputManager.BuildStructureRotation.performed += RotateStrucurePerformed;

        _gameManager.OnGameStateChanged += ChangeBuildAvailable;
    }

    private void Awake()
    {
        _camera = Camera.main;
        _structurePivotLayer = LayerMask.NameToLayer("StructurePivot");
    }

    private void Update()
    {
        if(_enabled && _currentStructurePreview)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Vector3 previewPosition = Vector3.zero;
            Quaternion previewRotation = _previewRotation;

            if (Physics.Raycast(ray, out var hit, PreviewDistance, LayerMask))
            {
                Vector3 previewOffset = Vector3.Scale(hit.normal, _currentStructurePreview.transform.localScale) / 2f;
                previewPosition = hit.point + previewOffset;
                if (hit.collider.gameObject.layer == _structurePivotLayer)
                {
                    var pivotInfo = hit.collider.GetComponent<StructurePivotInfo>();
                    Vector3 direction = pivotInfo.Direction;
                    previewRotation = pivotInfo.Structure.transform.rotation;
                    if (_currentStructureInfo.Structure is Floor)
                    {
                        if(pivotInfo.Structure is Floor)
                        {
                            direction.y = 0f;
                            Vector3 worldDirection = pivotInfo.Structure.transform.TransformDirection(direction);
                            previewOffset = Vector3.Scale(worldDirection, _currentStructurePreview.transform.localScale);
                        }
                        else if(pivotInfo.Structure is Wall)
                        {
                            direction.x = 0f;
                            Vector3 worldDirection = pivotInfo.Structure.transform.TransformDirection(direction);
                            previewOffset = Vector3.Scale(worldDirection, _currentStructurePreview.transform.localScale / 2f);
                            previewOffset.y = pivotInfo.Direction.y * pivotInfo.Structure.transform.localScale.y / 2f -
                                _currentStructurePreview.transform.localScale.y / 2f;
                        }
                    } else if(_currentStructureInfo.Structure is Wall)
                    {
                        if (pivotInfo.Structure is Wall)
                        {
                            direction.z = 0f;
                            if (direction.x != 0f && direction.y != 0f)
                            {
                                direction.y = 0f;
                            }
                            Vector3 worldDirection = pivotInfo.Structure.transform.TransformDirection(direction);
                            previewOffset = Vector3.Scale(worldDirection, _currentStructurePreview.transform.localScale);
                            previewOffset.x = worldDirection.x * _currentStructurePreview.transform.localScale.x;
                            previewOffset.z = worldDirection.z * _currentStructurePreview.transform.localScale.x;
                        } else if(pivotInfo.Structure is Floor)
                        {
                            Vector3 worldDirection = pivotInfo.Structure.transform.TransformDirection(direction);
                            previewOffset = Vector3.Scale(worldDirection, pivotInfo.Structure.transform.localScale / 2f);
                            previewOffset.y = pivotInfo.Direction.y * _currentStructurePreview.transform.localScale.y / 2f + 
                                pivotInfo.Structure.transform.localScale.y / 2f;
                            if(pivotInfo.Direction.x != 0f)
                            {
                                previewRotation = Quaternion.Euler(previewRotation.eulerAngles + new Vector3(0f, 90 * pivotInfo.Direction.x, 0f));
                            }
                        }
                    }

                    previewPosition = pivotInfo.Structure.transform.position + previewOffset;
                }
            } else
            {
                previewPosition = ray.origin + ray.direction * PreviewDistance;
            }
            _currentStructurePreview.transform.position = previewPosition;
            _currentStructurePreview.transform.rotation = previewRotation;
        }
    }

    private void RotateStructure(int direction)
    {
        Vector3 rotation = direction > 0 ? new Vector3(0f, 15f, 0f) : new Vector3(0f, -15f, 0f);
        _previewRotation = Quaternion.Euler(_previewRotation.eulerAngles + rotation);
    }

    private void TryPlaceStructure(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        var prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

        var structure = Instantiate(prefab, position, rotation, transform);

        structure.name = $"{prefab.name}[{_structureID}]";
        _structureID++;

        NetworkServer.Spawn(structure);
    }

    [Command(requiresAuthority = false)]
    private void CmdPlaceStructure(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        TryPlaceStructure(prefabIndex, position, rotation);
    }

    private void PlaceStructurePerformed(InputAction.CallbackContext callback)
    {
        if(!_enabled || !_currentStructureInfo || !_currentStructurePreview || !_currentStructurePreview.AvailableForPlace) return;

        int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(_currentStructureInfo.Structure.gameObject);
        if (NetworkServer.active)
        {
            TryPlaceStructure(prefabIndex, _currentStructurePreview.transform.position, _currentStructurePreview.transform.rotation);
        }
        else
        {
            CmdPlaceStructure(prefabIndex, _currentStructurePreview.transform.position, _currentStructurePreview.transform.rotation);
        }
    }

    private void RotateStrucurePerformed(InputAction.CallbackContext callback)
    {
        RotateStructure((int)callback.ReadValue<Vector2>().y);
    }

    private void ChangeBuildAvailable(GameState state)
    {
        _enabled = state == GameState.Build;

        if (_currentStructurePreview)
        {
            _currentStructurePreview.gameObject.SetActive(_enabled);
        }
    }

    private void ChangePreview()
    {
        if (_currentStructurePreview)
        {
            Destroy(_currentStructurePreview.gameObject);
            _currentStructurePreview = null;
        }

        _currentStructurePreview = Instantiate(CurrentStructureInfo.StructurePreview, transform);
        _currentStructurePreview.gameObject.SetActive(_enabled);
    }

    private void OnDisable()
    {
        _inputManager.BuildPlaceStructure.performed -= PlaceStructurePerformed;
        _inputManager.BuildStructureRotation.performed -= RotateStrucurePerformed;

        _gameManager.OnGameStateChanged -= ChangeBuildAvailable;
    }
}
