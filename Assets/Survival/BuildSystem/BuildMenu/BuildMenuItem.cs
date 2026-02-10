using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenuItem : MonoBehaviour, IPointerUpHandler
{
    public event Action<StructureInfo> OnStructureInfoChose;

    public StructureInfo _structureInfo;
    public TMP_Text _structureName;

    private void Awake()
    {
        _structureName.text = _structureInfo.Structure.name;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnStructureInfoChose?.Invoke(_structureInfo);
        }
    }
}
