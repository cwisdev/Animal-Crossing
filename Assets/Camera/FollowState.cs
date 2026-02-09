using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/States/Follow")]
public class FollowCamState : CameraState
{
    public Vector3 defaultDirection = Vector3.back;
    public float defaultZoom = 0.4f;

    public float maxDistance = 9.5f;
    public float minDistance = 4.5f;

    public float zoomSpeed = .1f;

    Vector3 direction;

    float zoom;
    float zoomVelocity;

    public override void Enter(CameraContext context)
    {
        if (direction.Equals(Vector3.zero))
            direction = defaultDirection;
        if (zoom == 0)
            zoom = defaultZoom;
    }

    public override void Exit(CameraContext context)
    {
    }

    public override void Tick(CameraContext context)
    {
        if (zoomVelocity != 0)
        {
            zoom += zoomVelocity * Time.deltaTime;
            zoom = Mathf.Clamp(zoom, 0, 1);
        }
        float height = GetHeight();
        float distance = GetDistance();
        float pitch = GetPitch();

        context.camera.position = context.target.position + (direction * distance);
        context.camera.position += Vector3.up * height;
        context.camera.LookAt(context.target.position + (Vector3.up * (height + pitch)));
    }

    public override void Rotate(CameraContext context, float amount)
    {
    }

    public override void Zoom(CameraContext context, float amount)
    {
        zoomVelocity = amount * zoomSpeed;

    }

    private float GetDistance()
    {
        return (maxDistance - minDistance) * zoom + minDistance;
    }
    private float GetPitch()
    {
        return 4.1667f * Mathf.Pow(zoom, 2) - 9.1667f * zoom;
    }

    private float GetHeight()
    {
        return 1.0417f * Mathf.Pow(zoom, 2) + 3.2083f * zoom + 1.75f;
    }
}