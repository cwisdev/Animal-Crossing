using UnityEngine;

public struct CameraState
{
    public Vector3 position;
    public Quaternion rotation;

    public CameraState(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}