using UnityEngine;
using static System.TimeZoneInfo;

public class CameraTransition
{
    Vector3 startPosition, targetPosition;
    Quaternion startRotation, targetRotation;

    float duration;
    float timer;
    bool transitioning = false;

    public void Begin(CameraState startState, CameraState targetState, float duration)
    {
        startPosition = startState.position;
        startRotation = startState.rotation;
        targetPosition = targetState.position;
        targetRotation = targetState.rotation;

        Debug.Log("start:  " + startRotation);
        Debug.Log("target: " + targetRotation);
        
        this.duration = duration;
        timer = 0;
        transitioning = true;
    }

    public void UpdateTransition(Transform camera)
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        //t = t * t * (3 - 2 * t);

        camera.position = Vector3.Lerp(startPosition, targetPosition, t);
        camera.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
        Debug.Log(camera.rotation + "/" + t + "/" + timer);

        if (t >= 1f)
            transitioning = false;
    }

    public bool IsTransitioning()
    {
        return transitioning;
    }
}