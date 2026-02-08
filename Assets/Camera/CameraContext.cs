using System;
using UnityEngine;

[Serializable]
public class CameraContext
{
    public Transform camera;
    public Transform target;

    public CameraContext(Transform camera, Transform target)
    {
        this.camera = camera;
        this.target = target;
    }
}