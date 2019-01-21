using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sf = UnityEngine.SerializeField;

[ExecuteInEditMode]
public class PositionByLine : MonoBehaviour
{
    public LineRenderer line;
    public int index;
    public Vector3 offset;

    private void Update()
    {
        if (line == null || line.positionCount <= index) return;
        transform.localPosition = line.GetPosition(index) + (Vector3)offset;
    }
}
