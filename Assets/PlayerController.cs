using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MovementController movement;

    private void Start()
    {
        movement = GetComponent<MovementController>();
    }

    private void OnMove(InputValue value)
    {
        movement.SetMovement(value.Get<Vector2>());
        //Debug.Log("Controller: " + value.Get<Vector2>());
    }
}
