using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    private void OnDrawGizmos()
    {
        var cellSize = _grid.cellSize;
        var cellGap = _grid.cellGap;
        var origin = _grid.transform.position;

        Gizmos.color = Color.red;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                var pos = origin + new Vector3(x * (cellSize.x + cellGap.x), 0, z * (cellSize.z + cellGap.z));

                Gizmos.DrawLine(pos, pos + new Vector3(cellSize.x, 0, 0));
                Gizmos.DrawLine(pos, pos + new Vector3(0, 0, cellSize.z));
                Gizmos.DrawLine(pos + new Vector3(cellSize.x, 0, 0), pos + new Vector3(cellSize.x, 0, cellSize.z));
                Gizmos.DrawLine(pos+new Vector3(0,0,cellSize.z),pos+new Vector3(cellSize.x,0,cellSize.z));
                
            }
        }
    }

}
