using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CameraBehaviour defaultBehaviour;
    public GameObject player;
    public float transitionSpeed = 1;
    
    CameraContext context;
    CameraTransition transition = new CameraTransition();
    CameraBehaviour behaviour;
    CameraState state;

    CameraBehaviour prevBehaviour;
    public GameObject secondObj;

    void Awake()
    {
        context = new CameraContext(transform, player.transform);
        defaultBehaviour.target = player;
        SetBehaviour(defaultBehaviour);
    }

    public void SetBehaviour(CameraBehaviour newBehaviour)
    {
        if (behaviour != null)
            behaviour.Exit();
        behaviour = newBehaviour;
        behaviour.Enter();
    }

    void LateUpdate()
    {

        if (transition.IsTransitioning())
        {
            state = transition.GetCurrentState();
            transition.SetTargetState(behaviour.GetCameraState(context));
            transition.UpdateTransition();
        }
        else
        {
            state = behaviour.GetCameraState(context);
        }

        transform.position = state.position;
        transform.rotation = state.rotation;
    }

    private void OnRotate(InputValue value)
    {
        if (!transition.IsTransitioning())
            behaviour.Rotate(value.Get<float>());
    }

    private void OnZoom(InputValue value)
    {
        if (!transition.IsTransitioning())
            behaviour.Zoom(value.Get<float>());
    }

    private void OnSwapCamera(InputValue value)
    {
        CameraState prevState = state;

        if (prevBehaviour == null)
        {
            prevBehaviour = ScriptableObject.CreateInstance<FollowBehaviour>();
            prevBehaviour.target = secondObj;

        }
        CameraBehaviour inbetweenBehaviour;
        inbetweenBehaviour = behaviour;
        SetBehaviour(prevBehaviour);
        prevBehaviour = inbetweenBehaviour;

        transition.Begin(prevState, state, transitionSpeed);
    }
}
