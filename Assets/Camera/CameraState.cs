using System;
using UnityEngine;

public abstract class CameraState : ScriptableObject
{
    // TODO: make this constants
    public float ZoomTolerance = .001f;
    public float RotateTolerance = 0.1f;
    
    public float zoomSpeed;
    public float rotateSpeed;

    // Target values
    [NonSerialized]
    private float targetZoom;
    [NonSerialized]
    private float targetYaw;

    // Current values
    [NonSerialized]
    private float zoom;
    [NonSerialized]
    private float yaw;


    // Transitions
    [NonSerialized]
    private float zoomProgress = 0;
    [NonSerialized]
    private float rotateProgress = 0;

    public abstract void Enter(CameraContext context);
    public abstract void Exit(CameraContext context);
    public abstract void Tick(CameraContext context);

    public abstract void Rotate(CameraContext context, float amount);
    public abstract void Zoom(CameraContext context, float amount);

    public void ImplicitTick(CameraContext context)
    {
        if (IsZooming())
        {
            zoomProgress += zoomSpeed * Time.deltaTime;
            if (!IsZooming())
                zoomProgress = 1;
            zoom = Mathf.Lerp(zoom, targetZoom, zoomProgress);
        }

        if (IsRotating())
        {
            rotateProgress += rotateSpeed * Time.deltaTime;
            if (!IsRotating())
            {
                rotateProgress = 1;
                targetYaw %= 360;
                yaw %= 360;
            }
            yaw = Mathf.Lerp(yaw, targetYaw, rotateProgress);
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
        zoomProgress = 0;
    }

    protected float GetTargetZoom()
    {
        return targetZoom;
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
        this.targetYaw = targetYaw;
        rotateProgress = 0;
    }

    protected float GetTargetYaw()
    {
        return targetYaw;
    }

    public void IncreaseTargetYaw(float amount)
    {
        SetTargetYaw(targetYaw + amount);
    }

    public bool IsZooming()
    {
        return 1 - zoomProgress > ZoomTolerance;
    }

    public bool IsRotating()
    {
        return 1 - rotateProgress > ZoomTolerance;
    }
}
