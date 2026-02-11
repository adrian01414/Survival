using Mirror;
using ModestTree;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BuildSystem : NetworkBehaviour
{
    public event Action<StructureInfo> OnStructurePlaced;
    public event Action<Structure> OnStructureDestroyed;

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
    private Structure _currentSelectedStructure = null;
    private Camera _camera;
    private int _structurePivotLayer;
    private Quaternion _previewRotation = Quaternion.identity;

    private bool _enabled = false;
    private bool _availableForPlace = true;

    private InputManager _inputManager;
    private GameManager _gameManager;
    private ResourceBank _resourceBank;

    [Inject]
    public void Construct(InputManager inputManager, GameManager gameManager, ResourceBank resourceBank)
    {
        _inputManager = inputManager;
        _gameManager = gameManager;
        _resourceBank = resourceBank;
    }

    private void OnEnable()
    {
        _inputManager.BuildPlaceStructure.performed += PlaceStructurePerformed;
        _inputManager.BuildStructureRotation.performed += RotateStrucurePerformed;
        _inputManager.BuildDestroyStructure.performed += DestroyStructurePerformed;

        _gameManager.OnGameStateChanged += ChangeBuildAvailable;
    }

    private void Awake()
    {
        _camera = Camera.main;
        _structurePivotLayer = LayerMask.NameToLayer("StructurePivot");
    }

    private void Update()
    {
        SetPreviewPositionAndRotation();
        CheckAvailableForPlace();
    }

    private void CheckAvailableForPlace()
    {
        if (!_currentStructureInfo) return;
        bool resourceAvailable = true;
        foreach(var cost in _currentStructureInfo.Cost)
        {
            if (_resourceBank.Resources[cost.ResourceType] - cost.Amount < 0)
            {
                resourceAvailable = false;
                break;
            }
        }
        _availableForPlace = resourceAvailable;
        _currentStructurePreview.ChangePreviewMaterial(resourceAvailable);
    }

    private Dictionary<Collider, StructurePivotInfo> pivotInfoCache = new Dictionary<Collider, StructurePivotInfo>();
    private StructurePivotInfo GetCachedPivotInfo(Collider collider)
    {
        if (!pivotInfoCache.TryGetValue(collider, out var pivotInfo))
        {
            pivotInfo = collider.GetComponent<StructurePivotInfo>();
            if (pivotInfo != null)
            {
                pivotInfoCache[collider] = pivotInfo;
            }
        }
        return pivotInfo;
    }

    private void SetPreviewPositionAndRotation()
    {
        if (!_enabled) return;

        Vector3 previewPosition = Vector3.zero;
        Quaternion previewRotation = _previewRotation;

        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, PreviewDistance, LayerMask))
        {
            Vector3 previewOffset = Vector3.Scale(hit.normal, _currentStructurePreview.transform.localScale) / 2f;
            previewPosition = hit.point + previewOffset;
            if (hit.collider.gameObject.layer == _structurePivotLayer)
            {
                var pivotInfo = GetCachedPivotInfo(hit.collider);

                if (pivotInfo.Structure != _currentSelectedStructure)
                {
                    _currentSelectedStructure = pivotInfo.Structure;
                }

                previewRotation = pivotInfo.Structure.transform.rotation;
                previewOffset = pivotInfo.Structure.CalculatePreviewOffset(_currentStructureInfo.Structure, pivotInfo);

                if (_currentStructureInfo.Structure is Wall && pivotInfo.Structure is Floor && pivotInfo.Direction.x != 0f)
                {
                    previewRotation = Quaternion.Euler(previewRotation.eulerAngles + new Vector3(0f, 90 * pivotInfo.Direction.x, 0f));
                }

                previewPosition = pivotInfo.Structure.transform.position + previewOffset;
            }
        }
        else
        {
            _currentSelectedStructure = null;
            previewPosition = ray.origin + ray.direction * PreviewDistance;
        }

        _currentStructurePreview.transform.position = previewPosition;
        _currentStructurePreview.transform.rotation = previewRotation;
    }

    private void RotateStructure(int direction)
    {
        Vector3 rotation = direction > 0 ? new Vector3(0f, 15f, 0f) : new Vector3(0f, -15f, 0f);
        _previewRotation = Quaternion.Euler(_previewRotation.eulerAngles + rotation);
    }

    private void PlaceStructure(int prefabIndex, Vector3 position, Quaternion rotation)
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
        PlaceStructure(prefabIndex, position, rotation);
    }

    private void PlaceStructurePerformed(InputAction.CallbackContext callback)
    {
        if(!_enabled || !_currentStructureInfo || !_currentStructurePreview || !_availableForPlace) return;

        int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(_currentStructureInfo.Structure.gameObject);
        if (NetworkServer.active)
        {
            PlaceStructure(prefabIndex, _currentStructurePreview.transform.position, _currentStructurePreview.transform.rotation);
        }
        else
        {
            CmdPlaceStructure(prefabIndex, _currentStructurePreview.transform.position, _currentStructurePreview.transform.rotation);
        }

        OnStructurePlaced?.Invoke(CurrentStructureInfo);
    }

    private void RemoveStructure(uint netId)
    {
        var structure = NetworkServer.spawned[netId].gameObject;

        Destroy(structure);
        
        NetworkServer.UnSpawn(structure);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveStructure(uint netId)
    {
        RemoveStructure(netId);
    }

    private void DestroyStructurePerformed(InputAction.CallbackContext callback)
    {
        if (!_enabled) return;

        uint netid = _currentSelectedStructure.GetComponent<NetworkIdentity>().netId;

        if (NetworkServer.active)
        {
            RemoveStructure(netid);
        }
        else
        {
            CmdRemoveStructure(netid);
        }

        OnStructureDestroyed?.Invoke(_currentSelectedStructure);
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
        }

        _currentStructurePreview = Instantiate(CurrentStructureInfo.StructurePreview, transform);
        _currentStructurePreview.gameObject.SetActive(_enabled);
    }

    private void OnDisable()
    {
        _inputManager.BuildPlaceStructure.performed -= PlaceStructurePerformed;
        _inputManager.BuildStructureRotation.performed -= RotateStrucurePerformed;
        _inputManager.BuildDestroyStructure.performed -= DestroyStructurePerformed;

        _gameManager.OnGameStateChanged -= ChangeBuildAvailable;
    }
}
