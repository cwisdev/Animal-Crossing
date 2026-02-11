using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CameraState startingState;
    public GameObject player;

    Transform targetTransform;

    CameraContext context;
    CameraState state;

    void Awake()
    {
        context = new CameraContext(transform, player.transform);
        SetState(startingState);
    }

    public void SetState(CameraState newState)
    {
        if (state != null)
            state.Exit(context);
        state = newState;
        state.Enter(context);
    }

    void LateUpdate()
    {
        state.ImplicitTick(context);
        state.Tick(context);
    }

    private void OnRotate(InputValue value)
    {
        state.Rotate(context, value.Get<float>());
    }

    private void OnZoom(InputValue value)
    {
        state.Zoom(context, value.Get<float>());
    }
}
