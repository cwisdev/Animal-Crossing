using UnityEngine;

public abstract class CameraState : ScriptableObject
{

    public abstract void Enter(CameraContext context);
    public abstract void Exit(CameraContext context);
    public abstract void Tick(CameraContext context);

    public abstract void Rotate(CameraContext context, float amount);
}
