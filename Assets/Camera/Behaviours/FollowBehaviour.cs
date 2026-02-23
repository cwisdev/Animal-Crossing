using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Behaviours/Follow Behaviour")]
public class FollowBehaviour : CameraBehaviour
{
    public float distance = 9;
    public float defaultYaw = 90;
    public float rotateThreshold = 30;

    public float maxRotateDuration = 1;
    public float minRotateDuration = .5f;
    public float maxZoomDuration = 1;
    public float minZoomDuration = .5f;

    //[NonSerialized]
    public float yaw = -1;
    //[NonSerialized]
    public float lastYaw;
    //[NonSerialized]
    public float targetYaw;
    //[NonSerialized]
    public float rotateTimer;
    //[NonSerialized]
    public float rotateDuration;
    //[NonSerialized]
    bool rotating;

    //[NonSerialized]
    public int zoomLevel;
    //[NonSerialized]
    public float height = 5;
    //[NonSerialized]
    public float lookHeight = 0;
    //[NonSerialized]
    public float lastHeight;
    //[NonSerialized]
    public float lastLookHeight;
    //[NonSerialized]
    public float targetHeight;
    //[NonSerialized]
    public float targetLookHeight;
    //[NonSerialized]
    public float zoomTimer;
    //[NonSerialized]
    public float zoomDuration;
    //[NonSerialized]
    bool zooming;
    public int lastZoomDirection = 1;

    static ZoomState CloseZoom = new ZoomState(3, -3);
    static ZoomState MidZoom = new ZoomState(5, 0);
    static ZoomState FarZoom = new ZoomState(9, 0);
    static ZoomState[] zoomStates = { CloseZoom, MidZoom, FarZoom };

    public override void Enter()
    {
        if (yaw == -1)
            targetYaw = defaultYaw;
        if (zoomLevel == 0)
            SetZoomLevel(1);
        ResetTargetYaw();
        ResetTargetZoom();
    }

    public override void Exit()
    {
    }

    public override CameraState GetCameraState(CameraContext context)
    {
        if (rotating)
            UpdateRotation();
        if (zooming)
            UpdateZoom();

        Vector3 direction = Quaternion.Euler(0, yaw, 0) * Vector3.forward;

        Vector3 position = context.target.position + direction * distance;
        position += Vector3.up * height;

        Vector3 lookPos = Vector3.up * lookHeight;
        Quaternion rotation = Quaternion.LookRotation(context.target.position - (position + lookPos));

        return new CameraState(position, rotation);
    }

    public override void Rotate(float amount)
    {
        if (!zooming)
        {
            float inbetweenYaw = Mathf.Round(yaw / 90) * 90;
            float? newTargetYaw = null;

            if (amount > 0)
            {
                if (inbetweenYaw > targetYaw)
                    newTargetYaw = inbetweenYaw;
                else if (targetYaw - inbetweenYaw < rotateThreshold)
                    newTargetYaw = inbetweenYaw + 90;
            }
            else if (amount < 0)
            {
                if (inbetweenYaw < targetYaw)
                    newTargetYaw = inbetweenYaw;
                else if (inbetweenYaw - targetYaw < rotateThreshold)
                    newTargetYaw = inbetweenYaw - 90;
            }

            if (newTargetYaw != null)
            {
                lastYaw = yaw;
                rotateTimer = 0;
                targetYaw = (float) newTargetYaw;
                rotateDuration = Mathf.Clamp01(Mathf.Abs(targetYaw - yaw) / 90) * maxRotateDuration;
                if (rotateDuration < minRotateDuration)
                    rotateDuration = minRotateDuration;
                rotating = true;
            }
        }
    }

    public override void Zoom(float amount)
    {
        if (!rotating)
        {
            if ((amount > 0) && (zoomLevel < zoomStates.Length-1))
                SetZoomLevel(zoomLevel + 1);
            if ((amount < 0) && (zoomLevel > 0))
                SetZoomLevel(zoomLevel - 1);
        }
    }

    void SetZoomLevel(int level)
    {
        int zoomDirection = level - zoomLevel;

        lastHeight = height;
        lastLookHeight = lookHeight;

        ZoomState zoomState = zoomStates[level];
        targetHeight = zoomState.height;
        targetLookHeight = zoomState.offset;

        zoomDuration = maxZoomDuration;
        if (zooming && (zoomDirection != lastZoomDirection))
            if ((zoomDuration != 0) && (zoomTimer != 0))
                zoomDuration = Mathf.Max((zoomTimer / zoomDuration) * maxZoomDuration, minZoomDuration);

        lastZoomDirection = zoomDirection;
        zoomLevel = level;
        zoomTimer = 0;
        zooming = true;
    }

    void UpdateRotation()
    {
        if (rotateTimer < rotateDuration)
        {
            rotateTimer += Time.deltaTime;
            float t = Mathf.Clamp01(rotateTimer / rotateDuration);
            t = t * t * (3 - 2 * t);
            yaw = Mathf.LerpAngle(lastYaw, targetYaw, t);
        }
        else
            ResetTargetYaw();
    }

    void UpdateZoom()
    {
        if (zoomTimer < zoomDuration)
        {
            zoomTimer += Time.deltaTime;
            float t = Mathf.Clamp01(zoomTimer / zoomDuration);
            t = t * t * (3 - 2 * t);
            height = Mathf.Lerp(lastHeight, targetHeight, t);
            lookHeight = Mathf.Lerp(lastLookHeight, targetLookHeight, t);
        }
        else
            ResetTargetZoom();
    }

    void ResetTargetYaw()
    {
        targetYaw %= 360;
        yaw = targetYaw;
        rotating = false;
        rotateTimer = 0;
        rotateDuration = 0;
    }

    void ResetTargetZoom()
    {
        height = targetHeight;
        lookHeight = targetLookHeight;
        zooming = false;
        zoomTimer = 0;
        zoomDuration = 0;
    }
}

public struct ZoomState
{
    public float height;
    public float offset;

    public ZoomState(float height, float offset)
    {
        this.height = height;
        this.offset = offset;
    }
}