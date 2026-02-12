using UnityEngine;

public class Wall : Structure
{
    public override Vector3 CalculatePreviewOffset(Structure preview, StructurePivotInfo pivotInfo)
    {
        Vector3 result = Vector3.zero;
        var direction = pivotInfo.Direction;

        if (preview is Wall)
        {
            direction.z = 0f;
            if (direction.x != 0f && direction.y != 0f)
            {
                direction.y = 0f;
            }
            Vector3 worldDirection = transform.TransformDirection(direction);
            result = Vector3.Scale(worldDirection, preview.SizePivot.transform.localScale);
            result.x = worldDirection.x * preview.SizePivot.transform.localScale.x;
            result.z = worldDirection.z * preview.SizePivot.transform.localScale.x;
        }
        else if (preview is Floor)
        {
            direction.x = 0f;
                Vector3 worldDirection = transform.TransformDirection(direction);
                result = Vector3.Scale(worldDirection, preview.SizePivot.transform.localScale / 2f);
                result.y = direction.y * SizePivot.transform.localScale.y / 2f -
                    preview.SizePivot.transform.localScale.y / 2f;
        }

        return result;
    }
}
