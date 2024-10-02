using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Tilemaps;
using UnityEngine;

[CustomGridBrush(true, false, false, "GameObject Brush XZ")]
public class GameObjectBrushXZ : GameObjectBrush
{
    // GameObjectBrush의 코드를 y축 회전에 맞게 변경
    public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
    {
        var oldSize = size;
        var oldCells = cells.Clone() as BrushCell[];
        size = new Vector3Int(oldSize.z, oldSize.y, oldSize.x);
        var oldBounds = new BoundsInt(Vector3Int.zero, oldSize);

        foreach (Vector3Int oldPos in oldBounds.allPositionsWithin)
        {
            var newX = direction == RotationDirection.Clockwise ? oldSize.z - oldPos.z - 1 : oldPos.z;
            var newZ = direction == RotationDirection.Clockwise ? oldPos.x : oldSize.x - oldPos.x - 1;
            var toIndex = GetCellIndex(newX, oldPos.y, newZ);
            var fromIndex = GetCellIndex(oldPos.x, oldPos.y, oldPos.z, oldSize.x, oldSize.y, oldSize.z);
            cells[toIndex] = oldCells[fromIndex];
        }

        var newPivotX = direction == RotationDirection.Clockwise ? oldSize.z - pivot.z - 1 : pivot.z;
        var newPivotZ = direction == RotationDirection.Clockwise ? pivot.x : oldSize.x - pivot.x - 1;
        pivot = new Vector3Int(newPivotX, pivot.y, newPivotZ);

        Quaternion orientation = Quaternion.Euler(0f, direction != RotationDirection.Clockwise ? 90f : -90f, 0f);
        foreach (BrushCell cell in cells)
            cell.orientation = cell.orientation * orientation;
    }
}
