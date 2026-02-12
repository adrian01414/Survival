using UnityEngine;

public class Floor : Structure
{
    public override Vector3 CalculatePreviewOffset(Structure preview, StructurePivotInfo pivotInfo)
    {
        Vector3 result = Vector3.zero;

        if (pivotInfo.Structure != this) return result;

        var direction = pivotInfo.Direction;

        if (preview is Floor)
        {
            direction.y = 0f;
            Vector3 worldDirection = transform.TransformDirection(direction);
            result = Vector3.Scale(worldDirection, preview.SizePivot.transform.localScale);
        } else if (preview is Wall)
        {
            Vector3 worldDirection = transform.TransformDirection(direction);
            result = Vector3.Scale(worldDirection, SizePivot.transform.localScale / 2f);
            result.y = direction.y * preview.SizePivot.transform.localScale.y / 2f +
                SizePivot.transform.localScale.y / 2f;
        }

        return result;
    }
}
