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
        state.Tick(context);
    }

    private void OnRotate(InputValue value)
    {
        float rotateAmount = value.Get<float>();
        if (rotateAmount > 0)
            state.Rotate(context, 90);
        else if (rotateAmount < 0)
            state.Rotate(context, -90);
    }
}
