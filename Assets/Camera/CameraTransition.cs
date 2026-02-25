using System;
using UnityEngine;
using static System.TimeZoneInfo;

public class CameraTransition
{
    public static event Action OnTransitionBegin;
    public static event Action OnTransitionEnd;

    //Vector3 startPosition, targetPosition;
    //Quaternion startRotation, targetRotation;
    CameraState startState, targetState, currentState;

    float duration;
    float timer;
    bool transitioning = false;

    public void Begin(CameraState startState, CameraState targetState, float duration)
    {
        this.startState = startState;
        this.targetState = targetState;
        currentState = this.startState;

        //startPosition = startState.position;
        //startRotation = startState.rotation;
        //targetPosition = targetState.position;
        //targetRotation = targetState.rotation;
        
        this.duration = duration;
        timer = 0;
        transitioning = true;

        OnTransitionBegin?.Invoke();
    }
    
    public void SetTargetState(CameraState targetState)
    {
        this.targetState = targetState;
    }

    public void UpdateTransition()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        t = t * t * (3 - 2 * t);

        currentState.position = Vector3.Lerp(startState.position, targetState.position, t);
        currentState.rotation = Quaternion.Slerp(startState.rotation, targetState.rotation, t);
        // Cancel all z-axis rotation
        currentState.rotation = Quaternion.Euler(
            currentState.rotation.eulerAngles.x,
            currentState.rotation.eulerAngles.y,
            0
        );

        if (t >= 1f)
        {
            transitioning = false;
            OnTransitionEnd?.Invoke();
        }
    }

    public CameraState GetCurrentState()
    {
        return currentState;
    }

    public bool IsTransitioning()
    {
        return transitioning;
    }
}