using System;
using UnityEngine;

public class FollowPlayerState : CameraState
{
    [NonSerialized]
    public Vector3 direction = new Vector3(0, .5f, -1f);
    public float distance = 6;
    public float height = .65f;
    public float heightOffset = 0;
    public float rotateSpeed = 500;
    public float zoomSpeed = 5;

    [NonSerialized]
    float yaw = 0;
    [NonSerialized]
    float targetYaw = 0;
    [NonSerialized]
    float currDistance = 0;
    [NonSerialized]
    float currHeight = 0;
    [NonSerialized]
    float currHeightOffset = 0;
    [NonSerialized]
    ZoomLevel zoomLevel;


    enum ZoomLevel
    {
        Close,
        Normal,
        Far
    }

    public override void Enter(CameraContext context)
    {
    }

    public override void Exit(CameraContext context)
    {
    }

    public override void Tick(CameraContext context)
    {
        UpdateYaw();
        UpdateOffsets();

        context.camera.position = context.target.position + (direction * currDistance);
        context.camera.position += Vector3.up * currHeight;
        context.camera.LookAt(context.target.position + (Vector3.up * (currHeight + currHeightOffset)));
    }

    public override void Rotate(CameraContext context, float amount)
    {
        targetYaw = amount;
    }

    public override void Zoom(CameraContext context, float amount)
    {
        switch (zoomLevel)
        {
            case ZoomLevel.Close:
                if (amount < 0)
                    SetZoomLevel(ZoomLevel.Normal);
                break;
            case ZoomLevel.Normal:
                if (amount < 0)
                    SetZoomLevel(ZoomLevel.Far);
                else if (amount > 0)
                    SetZoomLevel(ZoomLevel.Close);
                break;
            case ZoomLevel.Far:
                if (amount > 0)
                    SetZoomLevel(ZoomLevel.Normal);
                break;

        }
    }

    private void SetZoomLevel(ZoomLevel zoomLevel)
    {
        this.zoomLevel = zoomLevel;
        switch (zoomLevel)
        {
            case ZoomLevel.Close:
                distance = 4.5f;
                height = .3f;
                heightOffset = 1.3f;
                break;
            case ZoomLevel.Normal:
                distance = 6.5f;
                height = .65f;
                heightOffset = 0;
                break;
            case ZoomLevel.Far:
                distance = 9.5f;
                height = 2;
                heightOffset = -1;
                break;
        }
    }

    // Orbit the camera until the target direction is reached.
    private void UpdateYaw()
    {
        if (yaw != targetYaw)
        {
            float deltaYaw = ClampDelta(yaw, targetYaw, rotateSpeed * Time.deltaTime);
            yaw += deltaYaw;
            direction = Quaternion.Euler(0, deltaYaw, 0) * direction;

            // Reset the yaw values so they are not constantly increasing
            if (yaw == targetYaw)
            {
                yaw = 0;
                targetYaw = 0;
            }
        }
    }

    // Pull the camera up/down and forward/backward until the target height and distance is reached.
    private void UpdateOffsets()
    {
        if (currDistance != distance)
            currDistance += ClampDelta(currDistance, distance, zoomSpeed * Time.deltaTime);

        if (currHeight != height)
            currHeight += ClampDelta(currHeight, height, zoomSpeed * Time.deltaTime);

        if (currHeightOffset != heightOffset)
            currHeightOffset += ClampDelta(currHeightOffset, heightOffset, zoomSpeed * Time.deltaTime * .1f);
    }

    // Returns a delta value that ensures the current value plus the delta does not overshoot the target.
    private float ClampDelta(float current, float target, float delta)
    {
        if ((current < target) && (current + delta > target))
            delta = target - current;
        else if (current > target)
        {
            delta *= -1;
            if (current - delta < target)
                delta = (current - target) * -1;
        }

        return delta;
    }
}