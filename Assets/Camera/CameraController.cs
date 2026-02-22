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

    void Awake()
    {
        context = new CameraContext(transform, player.transform);
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
        state = behaviour.GetCameraState(context);

        if (transition.IsTransitioning())
            transition.UpdateTransition(transform);
        else
        {
            transform.position = state.position;
            transform.rotation = state.rotation;
        }
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
}
