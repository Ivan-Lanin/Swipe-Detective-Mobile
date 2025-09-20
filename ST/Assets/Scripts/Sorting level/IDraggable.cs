using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{
    public bool isActive { get;}

    void StartDrag(Vector3 hitPoint);
    void DragObject(Ray ray);
    void EndDrag();
}
