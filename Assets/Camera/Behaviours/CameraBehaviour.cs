using UnityEngine;

public abstract class CameraBehaviour : ScriptableObject
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract CameraState GetCameraState(CameraContext context);
    public abstract void Rotate(float amount);
    public abstract void Zoom(float amount);
}
