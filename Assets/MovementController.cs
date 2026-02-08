using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MovementController : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveSpeed = 6;
    public float rotateSpeed = 18;
    public float friction = .2f;
    public Vector3 startingDirection = Vector3.back;
    public bool cameraRelative = false;

    private Rigidbody rb;
    private Vector3 targetDirection;
    private Vector3 moveDirection;
    private Vector3 currentDirection;
    private float dirHorizontal, dirVertical;
    private float moveAmount;
    private float moveVelocity;
    private float rotateVelocity;
    private bool isPivoting = false;
    private float lastDirectionSign = 1;

    // TODO: make these constants
    public float maxRotationDifference = 120;
    public float moveAcceleration = .33f;
    public float rotateAcceleration = .2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set the starting direction
        currentDirection = startingDirection;
        rb.rotation = Quaternion.LookRotation(currentDirection);
        transform.rotation = Quaternion.LookRotation(currentDirection);
    }

    void FixedUpdate()
    {
        float maxMoveVelocity = 0;

        if (moveAmount > 0.1)
        {
            UpdateTargetDirection();
            float directionDifference = Vector3.SignedAngle(currentDirection, targetDirection, Vector3.up);
            
            // Reset the rotational velocity if we are now turning in a different direction
            float currDirectionSign = Mathf.Sign(directionDifference);
            if (lastDirectionSign != currDirectionSign)
                rotateVelocity = 0;
            lastDirectionSign = currDirectionSign;

            // Increase rotational velocity
            if (rotateVelocity < rotateSpeed)
                rotateVelocity = Mathf.Min(rotateVelocity + (rotateSpeed * rotateAcceleration), rotateSpeed);

            // Rotate towards the target direction
            float rotateStep = rotateVelocity * Mathf.Deg2Rad * Time.deltaTime * 50;
            currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotateStep, 0);
            transform.rotation = Quaternion.LookRotation(currentDirection);

            maxMoveVelocity = moveSpeed * moveAmount;

            // If making a sharp turn, begin pivoting
            if (Mathf.Abs(directionDifference) > maxRotationDifference)
                isPivoting = true;
            else if (Mathf.Abs(directionDifference) < 5) // Stop pivoting once facing approx. the correct direction
                isPivoting = false;

            // If we are pivoting, halt movement
            if (isPivoting)
                maxMoveVelocity = 0;

            // Increase movement velocity
            if (moveVelocity < maxMoveVelocity)
                moveVelocity += moveSpeed * moveAcceleration * Time.deltaTime * 50;
        }
        else
        {
            // If movement stops, reenter the pivoting state
            isPivoting = true;
            rotateVelocity = 0;
        }

        if (moveVelocity > maxMoveVelocity)
        {
            // Decrease movement velocity according to friction
            if (isPivoting)
                moveVelocity -= moveSpeed * friction * Time.deltaTime * 50;
            else
                moveVelocity = maxMoveVelocity;

            if (moveVelocity < 0)
                moveVelocity = 0;
        }

        // Update the movement direction if we are not pivoting
        // This allows "skidding" when making sharp turns, i.e. prevents movement in an undesired
        // direction while pivoting
        if (!isPivoting)
            moveDirection = currentDirection;

        // Move in the current direction
        rb.linearVelocity = moveDirection * moveVelocity * Time.deltaTime * 50;
    }

    public void SetMovement(Vector2 value)
    {
        dirHorizontal = value.x;
        dirVertical = value.y;
        moveAmount = value.magnitude;
    }

    private void UpdateTargetDirection()
    {
        if (cameraRelative)
        {
            // Get the reference vectors from the camera
            Vector3 refForward = cameraTransform.forward;
            Vector3 refRight = cameraTransform.right;
            refForward.y = 0;
            refRight.y = 0;
            refForward.Normalize();
            refRight.Normalize();

            // Set the direction relative to the camera
            targetDirection = (refForward * dirVertical) + (refRight * dirHorizontal);
        }
        else
        {
            targetDirection.x = dirHorizontal;
            targetDirection.z = dirVertical;
        }

        targetDirection.y = 0;
        targetDirection.Normalize();
    }
}
