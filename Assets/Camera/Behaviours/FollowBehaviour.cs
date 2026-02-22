using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Behaviours/Follow Behaviour")]
public class FollowBehaviour : CameraBehaviour
{
    public float distance = 9;
    public float height = 5;
    public float lookHeight = 0;
    public float defaultYaw = 90;
    public float maxRotateDuration = 1;
    public float minRotateDuration = .5f;
    public float rotateThreshold = 30;

    //[NonSerialized]
    public float yaw = -1;
    public float lastYaw;
    //[NonSerialized]
    public float targetYaw;
    //[NonSerialized]
    public float rotateVelocity;
    //[NonSerialized]
    public float rotateTimer;
    //[NonSerialized]
    public float rotateDuration;
    bool rotating;

    public override void Enter()
    {
        if (yaw == -1)
            targetYaw = defaultYaw;
        ResetTargetYaw();
    }

    public override void Exit()
    {
    }

    public override CameraState GetCameraState(CameraContext context)
    {
        if (rotating)
            UpdateRotation();

        Vector3 direction = Quaternion.Euler(0, yaw, 0) * Vector3.forward;

        Vector3 position = context.target.position + direction * distance;
        position += Vector3.up * height;

        Vector3 lookPos = Vector3.up * lookHeight;
        Quaternion rotation = Quaternion.LookRotation(context.target.position - (position + lookPos));

        return new CameraState(position, rotation);
    }

    public override void Rotate(float amount)
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

    public override void Zoom(float amount)
    {
        throw new NotImplementedException();
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

    void ResetTargetYaw()
    {
        targetYaw %= 360;
        yaw = targetYaw;
        rotating = false;
        rotateTimer = 0;
        rotateDuration = 0;
    }
}