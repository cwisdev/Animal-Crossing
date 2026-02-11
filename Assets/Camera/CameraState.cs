using System;
using UnityEngine;

public abstract class CameraState : ScriptableObject
{
    // TODO: make this constants
    public float ZoomTolerance = .001f;
    public float RotateTolerance = 0.1f;
    
    public float zoomSpeed;
    public float rotateSpeed;

    // Desired values
    [NonSerialized]
    private float targetZoom;
    [NonSerialized]
    private float targetYaw;

    // Current values
    [NonSerialized]
    private float zoom;
    [NonSerialized]
    private float yaw;

    public abstract void Enter(CameraContext context);
    public abstract void Exit(CameraContext context);
    public abstract void Tick(CameraContext context);

    public abstract void Rotate(CameraContext context, float amount);
    public abstract void Zoom(CameraContext context, float amount);

    public void ImplicitTick(CameraContext context)
    {
        if (zoom != targetZoom)
        {
            zoom = Mathf.Lerp(zoom, targetZoom, zoomSpeed * Time.deltaTime);
            if (!IsZooming())
                zoom = targetZoom;
        }

        if (yaw != targetYaw)
        {
            yaw = Mathf.LerpAngle(yaw, targetYaw, rotateSpeed * Time.deltaTime);
            if (!IsRotating())
                yaw = targetYaw;
            yaw %= 360;
        }
    }

    public float GetZoom()
    {
        return zoom;
    }

    public void ForceZoom(float zoom)
    {
        SetTargetZoom(zoom);
        this.zoom = targetZoom;
    }

    public void SetTargetZoom(float targetZoom)
    {
        this.targetZoom = Mathf.Clamp(targetZoom, 0, 1);
    }

    public void IncreaseTargetZoom(float amount)
    {
        SetTargetZoom(targetZoom + amount);
    }

    public float GetYaw()
    {
        return yaw;
    }

    public void ForceYaw(float yaw)
    {
        SetTargetYaw(yaw);
        this.yaw = targetYaw;
    }

    public void SetTargetYaw(float targetYaw)
    {
        this.targetYaw = targetYaw % 360;
    }

    public void IncreaseTargetYaw(float amount)
    {
        SetTargetYaw(targetYaw + amount);
    }

    public bool IsZooming()
    {
        return Mathf.Abs(zoom - targetZoom) > ZoomTolerance;
    }

    public bool IsRotating()
    {
        return Mathf.Abs(Mathf.DeltaAngle(yaw, targetYaw)) > RotateTolerance;
    }
}
