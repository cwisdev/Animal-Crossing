using System;
using UnityEngine;

public abstract class CameraState : ScriptableObject
{
    public float zoomSpeed;

    // Desired values
    [NonSerialized]
    private float targetZoom;

    // Current values
    [NonSerialized]
    private float zoom;

    public abstract void Enter(CameraContext context);
    public abstract void Exit(CameraContext context);
    public abstract void Tick(CameraContext context);

    public abstract void Rotate(CameraContext context, float amount);
    public abstract void Zoom(CameraContext context, float amount);

    public void ImplicitTick(CameraContext context)
    {
        if (zoom != targetZoom)
            zoom = Mathf.Lerp(zoom, targetZoom, zoomSpeed * Time.deltaTime);
    }

    public float GetZoom()
    {
        return zoom;
    }

    public void ForceZoom(float zoom)
    {
        this.zoom = Mathf.Clamp(zoom, 0, 1);
        targetZoom = this.zoom;
        Debug.Log("forced zoom: " + targetZoom);
    }

    public void SetTargetZoom(float targetZoom)
    {
        this.targetZoom = Mathf.Clamp(targetZoom, 0, 1);
    }

    public void IncreaseTargetZoom(float amount)
    {
        SetTargetZoom(targetZoom + amount);
        Debug.Log(targetZoom);
    }

    public bool IsZooming()
    {
        return Mathf.Abs(zoom - targetZoom) > 0.01f;
    }
}
