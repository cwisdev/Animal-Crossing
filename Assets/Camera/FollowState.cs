using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/States/Follow")]
public class FollowCamState : CameraState
{
    public Vector3 defaultDirection = Vector3.back;
    public float defaultZoom = 0.4f;

    public float maxDistance = 9.5f;
    public float minDistance = 4.5f;
    
    public int numZoomLevels = 3;

    // TODO: convert to const
    public float RotateThreshold = 30;

    [NonSerialized]
    Vector3 direction;

    [NonSerialized]
    private float prevTargetYaw;

    public override void Enter(CameraContext context)
    {
        if (direction.Equals(Vector3.zero))
        {
            ForceYaw(0);
            prevTargetYaw = 0;
            direction = defaultDirection;
        }
        if (GetZoom() == 0)
            ForceZoom(defaultZoom);
    }

    public override void Exit(CameraContext context)
    {
    }

    public override void Tick(CameraContext context)
    {
        direction = Quaternion.Euler(0, GetYaw(), 0) * Vector3.forward;

        float height = GetHeight();
        float distance = GetDistance();
        float pitch = GetPitch();

        context.camera.position = context.target.position + (direction * distance);
        context.camera.position += Vector3.up * height;
        context.camera.LookAt(context.target.position + (Vector3.up * (height + pitch)));
    }

    public override void Rotate(CameraContext context, float amount)
    {
        if (amount != 0)
        {
            float inbetweenYaw = Mathf.Round(GetYaw() / 90) * 90;
            float newTargetYaw = 0;

            if (amount > 0)
            {
                if (inbetweenYaw > GetTargetYaw())
                    newTargetYaw = inbetweenYaw;
                else if (Mathf.Abs(GetTargetYaw() - inbetweenYaw) < RotateThreshold)
                    newTargetYaw = inbetweenYaw + 90;
            }
            else
            {
                if (inbetweenYaw < GetTargetYaw())
                    newTargetYaw = inbetweenYaw;
                else if (Mathf.Abs(inbetweenYaw - GetTargetYaw()) < RotateThreshold)
                    newTargetYaw = inbetweenYaw - 90;
            }

            if (newTargetYaw != 0)
                SetTargetYaw(newTargetYaw);
        }
    }

    public override void Zoom(CameraContext context, float amount)
    {
        if (amount != 0)
        {
            float zoomDelta = 1 / (float) numZoomLevels;
            if (amount < 0)
                zoomDelta *= -1;
            IncreaseTargetZoom(zoomDelta);
        }
    }

    private float GetDistance()
    {
        return (maxDistance - minDistance) * GetZoom() + minDistance;
    }

    private float GetPitch()
    {
        return 4.1667f * Mathf.Pow(GetZoom(), 2) - 9.1667f * GetZoom();
    }

    private float GetHeight()
    {
        return 1.0417f * Mathf.Pow(GetZoom(), 2) + 3.2083f * GetZoom() + 1.75f;
    }
}