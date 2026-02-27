using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Camera/Behaviours/Follow Behaviour")]
public class FollowBehaviour : CameraBehaviour
{
    public float distance = 9;
    public float defaultYaw = 90;
    // How many degrees the yaw increases/decreases by with one increment
    public float rotateAmount = 90;
    // Percentage that the current rotation must complete before additional rotations in the same direction can occur
    public float rotateThreshold = .66f;
    // Percentage that the current zoom must complete before additional zooming in the same direction can occur
    public float zoomThreshold = .8f;

    // Maximum rotation transition time in seconds
    public float maxRotateDuration = 1;
    // Minimum rotation transition time in seconds
    public float minRotateDuration = .5f;
    // Maximum zoom transition time in seconds
    public float maxZoomDuration = .8f;
    // Minimum rotation transition time in seconds
    public float minZoomDuration = .5f;

    [NonSerialized]
    float yaw = -1;
    [NonSerialized]
    float lastYaw;
    [NonSerialized]
    float targetYaw;
    [NonSerialized]
    float rotateTimer;
    [NonSerialized]
    float rotateDuration;
    [NonSerialized]
    bool rotating;

    [NonSerialized]
    int zoomLevel = -1;
    [NonSerialized]
    float height = 5;
    [NonSerialized]
    float lookHeight = 0;
    [NonSerialized]
    float lastHeight;
    [NonSerialized]
    float lastLookHeight;
    [NonSerialized]
    float targetHeight;
    [NonSerialized]
    float targetLookHeight;
    [NonSerialized]
    float zoomTimer;
    [NonSerialized]
    float zoomDuration;
    [NonSerialized]
    bool zooming;
    [NonSerialized]
    int lastZoomDirection = 1;

    static ZoomState CloseZoom = new ZoomState(3, -3);
    static ZoomState MidZoom = new ZoomState(5, 0);
    static ZoomState FarZoom = new ZoomState(9, 0);
    static ZoomState[] zoomStates = { CloseZoom, MidZoom, FarZoom };

    public override void Enter()
    {
        if (yaw == -1)
            targetYaw = defaultYaw;
        if (zoomLevel == -1)
            SetZoomLevel(1);

        ResetRotationTimer();
        ResetZoomTimer();
    }

    public override void Exit()
    {
        ResetRotationTimer();
        ResetZoomTimer();
    }

    public override CameraState GetCameraState(CameraContext context)
    {
        if (rotating)
            UpdateRotation();
        if (zooming)
            UpdateZoom();

        Vector3 direction = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
        Vector3 position = target.transform.position + direction * distance;
        position += Vector3.up * height;

        Vector3 lookPos = Vector3.up * lookHeight;
        Quaternion rotation = Quaternion.LookRotation(target.transform.position - (position + lookPos));

        return new CameraState(position, rotation);
    }

    public override void Rotate(float amount)
    {
        if (!zooming)
        {
            float inbetweenYaw = Mathf.Round(yaw / rotateAmount) * rotateAmount;
            float? newTargetYaw = null;

            if (amount > 0)
            {
                if (inbetweenYaw > targetYaw)
                    newTargetYaw = inbetweenYaw;
                else if (Mathf.Abs(yaw - targetYaw) < rotateThreshold)
                    newTargetYaw = inbetweenYaw + rotateAmount;
            }
            else if (amount < 0)
            {
                if (inbetweenYaw < targetYaw)
                    newTargetYaw = inbetweenYaw;
                else if (Mathf.Abs(yaw - targetYaw) < rotateThreshold)
                    newTargetYaw = inbetweenYaw - rotateAmount;
            }

            if (newTargetYaw != null)
                SetRotation((float)newTargetYaw);
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

    void SetRotation(float angle)
    {
        lastYaw = yaw;
        rotateTimer = 0;
        targetYaw = angle ;
        rotateDuration = Mathf.Clamp01(Mathf.Abs(targetYaw - yaw) / rotateAmount) * maxRotateDuration;
        if (rotateDuration < minRotateDuration)
            rotateDuration = minRotateDuration;
        rotating = true;
    }

    void SetZoomLevel(int level)
    {
        int zoomDirection = level - zoomLevel;

        // Don't allow additional changes to the zoom level until the previous transition has
        // passed the specified threshold.
        if (zooming && (zoomDirection == lastZoomDirection))
            if (zoomTimer / zoomDuration < zoomThreshold)
                return;

        lastHeight = height;
        lastLookHeight = lookHeight;

        ZoomState zoomState = zoomStates[level];
        targetHeight = zoomState.height;
        targetLookHeight = zoomState.offset;

        zoomDuration = maxZoomDuration;
        // Set the zoom transition duration appropriately if walking back from the previous target
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
            float t = UpdateAndGetTimer(ref rotateTimer, rotateDuration);
            yaw = Mathf.LerpAngle(lastYaw, targetYaw, t);
        }
        else
            ResetRotationTimer();
    }

    void UpdateZoom()
    {
        if (zoomTimer < zoomDuration)
        {
            float t = UpdateAndGetTimer(ref zoomTimer, zoomDuration);
            height = Mathf.Lerp(lastHeight, targetHeight, t);
            lookHeight = Mathf.Lerp(lastLookHeight, targetLookHeight, t);
        }
        else
            ResetZoomTimer();
    }

    void ResetRotationTimer()
    {
        targetYaw %= 360;
        yaw = targetYaw;
        rotateTimer = 0;
        rotateDuration = 0;
        rotating = false;
    }

    void ResetZoomTimer()
    {
        height = targetHeight;
        lookHeight = targetLookHeight;
        zoomTimer = 0;
        zoomDuration = 0;
        zooming = false;
    }

    float UpdateAndGetTimer(ref float timer, float duration)
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        t = t * t * (3 - 2 * t);
        return t;
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