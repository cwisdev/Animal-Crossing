using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/States/Follow")]
public class FollowPlayerState : CameraState
{
    [NonSerialized]
    public Vector3 direction = new Vector3(0, .5f, -1f);
    public float distance = 6;
    public float height = .65f;
    public float rotateSpeed = 5;

    [NonSerialized]
    float yaw = 0;
    [NonSerialized]
    float targetYaw = 0;

    public override void Enter(CameraContext context)
    {
    }

    public override void Exit(CameraContext context)
    {
    }

    public override void Tick(CameraContext context)
    {
        //direction = Vector3.RotateTowards(direction, targetDirection, rotateSpeed * Time.deltaTime, 0);
        if (yaw != targetYaw)
        {
            float deltaYaw = rotateSpeed * Time.deltaTime;
            if (yaw < targetYaw)
            {
                if (yaw + deltaYaw > targetYaw)
                    deltaYaw = targetYaw - yaw;
            }
            else
            {
                deltaYaw *= -1;
                if (yaw + deltaYaw < targetYaw)
                    deltaYaw = (yaw - targetYaw) * -1;
            }

            yaw += deltaYaw;
            direction = Quaternion.Euler(0, deltaYaw, 0) * direction;
            Debug.Log(yaw + "/" + targetYaw);
        }


        //context.camera.RotateAround(context.target.position, Vector3.up, rotateSpeed * Time.deltaTime);

        context.camera.position = context.target.position + (direction * distance);
        context.camera.position += Vector3.up * height;
        context.camera.LookAt(context.target.position + (Vector3.up * height * 2));
    }

    public override void Rotate(CameraContext context, float amount)
    {
        //targetDirection = Quaternion.Euler(0, amount, 0) * direction;
        targetYaw += amount;
    }
}