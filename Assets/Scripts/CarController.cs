using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody carRB;

    [SerializeField]
    private Transform[] rayPoints;

    [SerializeField]
    private LayerMask drivable;

    [SerializeField]
    private Transform accelerationPoint;

    [Header("Suspension settings")]
    [SerializeField]
    private float springStiffness;

    [SerializeField]
    private float damperStiffness;

    [SerializeField]
    private float restLength;

    [SerializeField]
    private float springTravel;

    [SerializeField]
    private float wheelRadius;

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("Car Settings")]
    [SerializeField]
    private float acceleration = 25f;

    [SerializeField]
    private float maxSpeed = 100f;

    [SerializeField]
    private float deceleration = 10f;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;

    #region Unity Functions
    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
    }
    #endregion

    #region Suspension Functions
    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            Transform rayPoint = rayPoints[i];
            float maxLenght = restLength + springTravel;

            if (
                Physics.Raycast(
                    rayPoint.position,
                    -rayPoint.up,
                    out RaycastHit hit,
                    maxLenght + wheelRadius,
                    drivable
                )
            )
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompresion = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(
                    carRB.GetPointVelocity(rayPoint.position),
                    rayPoint.up
                );
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompresion;

                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoint.up, rayPoint.position);

                Debug.DrawLine(rayPoint.position, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(
                    rayPoint.position,
                    rayPoint.position + (wheelRadius + maxLenght) * -rayPoint.up,
                    Color.green
                );
            }
        }
    }
    #endregion

    #region Input Handling
    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertcial");
        steerInput = Input.GetAxis("Horizontal");
    }
    #endregion

    #region Car Status Check
    private void GroundCheck()
    {
        int tempGroundedWheels = 0;
        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelIsGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    #endregion
}
