using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/States/Follow")]
public class FollowCamState : CameraState
{
    const int ZoomUp = 1;
    const int ZoomMid = 2;
    const int ZoomDown = 3;

    // TODO: convert to const
    public float RotateThreshold = 30;
    public float CloseOffset = 0;
    public float CloseHeight = 3;
    public float MidHeight = 5;
    public float FarHeight = 9;

    public int defaultZoomState = ZoomMid;
    public float distance = 9;

    private int zoomState;
    private float height;
    private float heightOffset;

    [NonSerialized]
    private float targetHeight;
    [NonSerialized]
    private float targetHeightOffset;
    [NonSerialized]
    private float zoomStateDelta;

    public void OnEnable()
    {
        SetZoomState(zoomState);
        height = targetHeight;
        heightOffset = targetHeightOffset;
    }
    public override void Enter(CameraContext context)
    {
        ForceYaw(180);
        if (zoomState == 0)
        {
            SetZoomState(defaultZoomState);
            height = targetHeight;
            heightOffset = targetHeightOffset;
        }
    }

    public override void Exit(CameraContext context)
    {
        height = targetHeight;
        heightOffset = targetHeightOffset;
    }

    public override void Tick(CameraContext context)
    {
        Vector3 direction = Quaternion.Euler(0, GetYaw(), 0) * Vector3.forward;

        if (IsChangingZoomState())
        {
            zoomStateDelta += zoomSpeed * Time.deltaTime;
            if (zoomStateDelta > zoomSpeed)
                zoomStateDelta = zoomSpeed;
            height = Mathf.Lerp(height, targetHeight, zoomStateDelta);
            heightOffset = Mathf.Lerp(heightOffset, targetHeightOffset, zoomStateDelta);

            if (!IsChangingZoomState())
            {
                height = targetHeight;
                heightOffset = targetHeightOffset;
                zoomStateDelta = 0;
            }
        }

        context.camera.position = context.target.position + (direction * distance);
        context.camera.position += Vector3.up * height;
        context.camera.LookAt(context.target.position + (Vector3.up *  heightOffset));
    }

    public override void Rotate(CameraContext context, float amount)
    {
        if (!IsChangingZoomState() && (amount != 0))
        {
            float inbetweenYaw = Mathf.Round(GetYaw() / 90) * 90;
            float newTargetYaw = -1;

            if (amount > 0)
            {
                if (inbetweenYaw > GetTargetYaw())
                    newTargetYaw = inbetweenYaw;
                else if (GetTargetYaw() - inbetweenYaw < RotateThreshold)
                    newTargetYaw = inbetweenYaw + 90;
            }
            else
            {
                if (inbetweenYaw < GetTargetYaw())
                    newTargetYaw = inbetweenYaw;
                else if (inbetweenYaw - GetTargetYaw() < RotateThreshold)
                    newTargetYaw = inbetweenYaw - 90;
            }

            if (newTargetYaw != -1)
                SetTargetYaw(newTargetYaw);
        }
    }

    public override void Zoom(CameraContext context, float amount)
    {
        if (!IsRotating())
        {
            if ((amount > 0) && (zoomState < 3))
                SetZoomState(zoomState + 1);
            if ((amount < 0) && (zoomState > 1))
                SetZoomState(zoomState - 1);
        }
    }

    private void SetZoomState(int zoomState)
    {
        switch (zoomState)
        {
            case ZoomUp:
                targetHeightOffset = CloseOffset;
                targetHeight = CloseHeight;
                break;
            case ZoomMid:
                targetHeightOffset = 0;
                targetHeight = MidHeight;
                break;
            case ZoomDown:
                targetHeightOffset = 0;
                targetHeight = FarHeight;
                break;

        }
        this.zoomState = zoomState;
        zoomStateDelta = 0;
    }

    private bool IsChangingZoomState()
    {
        return Mathf.Abs(heightOffset - targetHeightOffset) > ZoomTolerance
            || Mathf.Abs(height - targetHeight) > ZoomTolerance;
    }
}