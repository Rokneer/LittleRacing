using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;

    [SerializeField]
    private Transform[] rayPoints;

    [SerializeField]
    private LayerMask drivable;

    [SerializeField]
    private Transform accelerationPoint;

    [SerializeField]
    private GameObject[] tires = new GameObject[4];

    [SerializeField]
    private GameObject[] frontTiresParents = new GameObject[2];

    [SerializeField]
    private TrailRenderer[] skidMarks = new TrailRenderer[2];

    [SerializeField]
    private ParticleSystem[] skidSmokes = new ParticleSystem[2];

    [SerializeField]
    private AudioSource engineSound;

    [SerializeField]
    private AudioSource skidSound;

    [Header("Suspension Settings")]
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

    [SerializeField]
    private float steerStrength = 15f;

    [SerializeField]
    private AnimationCurve turningCurve;

    [SerializeField]
    private float dragCoefficient = 1f;

    [SerializeField]
    private float brakingDeceleration = 100f;

    [SerializeField]
    private float brakingDragCoefficient = 0.5f;

    private Vector3 currentCarLocalVelocity;
    private float carVelocityRatio;
    private readonly int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Visuals")]
    [SerializeField]
    private float tireRotSpeed = 3000f;

    [SerializeField]
    private float maxSteeringAngle = 30f;

    [SerializeField]
    private float minSideSkidVelocity = 10f;

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)]
    private float minPitch = 1;

    [SerializeField]
    [Range(1, 5)]
    private float maxPitch = 5;

    #region Unity Functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        Movement();
        Visuals();
        EngineSound();
    }
    #endregion

    #region Input Handling
    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }
    #endregion

    #region Movement
    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        if (currentCarLocalVelocity.z < maxSpeed)
        {
            rb.AddForceAtPosition(
                acceleration * moveInput * rb.transform.forward,
                accelerationPoint.position,
                ForceMode.Acceleration
            );
        }
    }

    private void Deceleration()
    {
        rb.AddForceAtPosition(
            (Input.GetKey(KeyCode.Space) ? brakingDeceleration : deceleration)
                * Mathf.Abs(carVelocityRatio)
                * -rb.transform.forward,
            accelerationPoint.position,
            ForceMode.Acceleration
        );
    }

    private void Turn()
    {
        rb.AddRelativeTorque(
            steerStrength
                * steerInput
                * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio))
                * Mathf.Sign(carVelocityRatio)
                * rb.transform.up,
            ForceMode.Acceleration
        );
    }

    private void SidewaysDrag()
    {
        float currenteSidewaysSpeed = currentCarLocalVelocity.x;

        float dragMagnitude =
            -currenteSidewaysSpeed
            * (Input.GetKey(KeyCode.Space) ? brakingDragCoefficient : dragCoefficient);

        Vector3 dragForce = transform.right * dragMagnitude;

        rb.AddForceAtPosition(dragForce, rb.worldCenterOfMass, ForceMode.Acceleration);
    }
    #endregion

    #region Visuals
    private void Visuals()
    {
        TireVisuals();
        Vfx();
    }

    private void TireVisuals()
    {
        float steeringAngle = maxSteeringAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i]
                    .transform
                    .Rotate(
                        Vector3.right,
                        tireRotSpeed * carVelocityRatio * Time.deltaTime,
                        Space.Self
                    );

                frontTiresParents[i].transform.localEulerAngles = new Vector3(
                    frontTiresParents[i].transform.localEulerAngles.x,
                    steeringAngle,
                    frontTiresParents[i].transform.localEulerAngles.z
                );
            }
            else
            {
                tires[i]
                    .transform
                    .Rotate(Vector3.right, tireRotSpeed * moveInput * Time.deltaTime, Space.Self);
            }
        }
    }

    private void Vfx()
    {
        if (
            isGrounded
            && Mathf.Abs(currentCarLocalVelocity.x) > minSideSkidVelocity
            && carVelocityRatio > 0
        )
        {
            ToggleSkidMarks(true);
            ToggleSkidSmokes(true);
            ToggleSkidSound(true);
        }
        else
        {
            ToggleSkidMarks(false);
            ToggleSkidSmokes(false);
            ToggleSkidSound(false);
        }
    }

    private void ToggleSkidMarks(bool toggle)
    {
        foreach (TrailRenderer skidMark in skidMarks)
        {
            skidMark.emitting = toggle;
        }
    }

    private void ToggleSkidSmokes(bool toggle)
    {
        foreach (ParticleSystem skidSmoke in skidSmokes)
        {
            if (toggle)
            {
                skidSmoke.Play();
            }
            else
            {
                skidSmoke.Stop();
            }
        }
    }

    private void SetTirePosition(GameObject tire, Vector3 targetPosition)
    {
        tire.transform.position = targetPosition;
    }
    #endregion

    #region Audio
    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocityRatio));
    }

    private void ToggleSkidSound(bool toggle)
    {
        skidSound.mute = !toggle;
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
        currentCarLocalVelocity = transform.InverseTransformDirection(rb.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    #endregion

    #region Suspension
    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            Transform rayPoint = rayPoints[i];
            float maxDistance = restLength;

            if (
                Physics.Raycast(
                    rayPoint.position,
                    -rayPoint.up,
                    out RaycastHit hit,
                    maxDistance + wheelRadius,
                    drivable
                )
            )
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompresion = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(rb.GetPointVelocity(hit.point), rayPoint.up);
                float damperForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompresion;

                float netForce = springForce - damperForce;

                rb.AddForceAtPosition(netForce * rayPoint.up, rayPoint.position);

                SetTirePosition(tires[i], hit.point + rayPoint.up * wheelRadius);

                Debug.DrawLine(rayPoint.position, hit.point, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;

                SetTirePosition(tires[i], rayPoint.position - rayPoint.up * maxDistance);

                Debug.DrawLine(
                    rayPoint.position,
                    rayPoint.position + (wheelRadius + maxDistance) * -rayPoint.up,
                    Color.green
                );
            }
        }
    }
    #endregion
}
