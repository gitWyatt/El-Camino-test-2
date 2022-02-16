using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public Transform centerOfMass;

    private Rigidbody carRigidBody;

    //I don't think I need this, might be leftover from old input system
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float transmissionForce;

    private float horizontalInput;
    private float verticalInput;
    private float gamepadVerticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private float brakeCheck;
    private float resetCheck;

    private bool isBraking;
    private bool isResetting;

    private bool touchingGround;
    private float distToGround = 0.65f;

    [SerializeField] private bool frontWheelDrive;
    [SerializeField] private bool rearWheelDrive;
    [SerializeField] private bool frontHandBrake;
    [SerializeField] private bool rearHandBrake;

    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private float controlPitchFactor;
    [SerializeField] private float controlYawFactor;
    [SerializeField] private float controlRollFactor;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    [SerializeField] InputAction steering;
    [SerializeField] InputAction gamepadVertical;
    [SerializeField] InputAction handbrake;
    [SerializeField] InputAction reset;

    private void Start()
    {
        //sets center of mass to transform
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = centerOfMass.localPosition;
    }

    private void OnEnable()
    {
        steering.Enable();
        gamepadVertical.Enable();
        handbrake.Enable();
        reset.Enable();
    }

    private void OnDisable()
    {
        steering.Disable();
        gamepadVertical.Disable();
        handbrake.Disable();
        reset.Disable();
    }

    private void FixedUpdate()
    {
        GetInput();
        CheckGround();
        HandleMotor();
        HandleSteering();
        HandleRotation();
        UpdateWheels();
        ResetCheck();
    }

    private void GetInput()
    {
        //horizontalInput = Input.GetAxis(HORIZONTAL);
        //verticalInput = Input.GetAxis(VERTICAL);
        //isBraking = Input.GetKey(KeyCode.Space);
        //idk maybe

        horizontalInput = steering.ReadValue<Vector2>().x;
        verticalInput = steering.ReadValue<Vector2>().y;
        gamepadVerticalInput = gamepadVertical.ReadValue<float>();
        brakeCheck = handbrake.ReadValue<float>();
        if (brakeCheck > .5)
        { isBraking = true; } else { isBraking = false; }
        resetCheck = reset.ReadValue<float>();
        if (resetCheck > .5)
        { isResetting = true; } else { isResetting = false; }
    }

    void CheckGround()
    {
        if (Physics.Raycast(frontLeftWheelTransform.localPosition, Vector3.down, distToGround) &&
            Physics.Raycast(frontRightWheelTransform.localPosition, Vector3.down, distToGround) &&
            Physics.Raycast(backLeftWheelTransform.localPosition, Vector3.down, distToGround) &&
            Physics.Raycast(backRightWheelTransform.localPosition, Vector3.down, distToGround)  )
        {
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
        Debug.Log(touchingGround);
    }

    private void HandleMotor()
    {
        if (frontWheelDrive)
        {
            //Transmission()
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;// * transmissionForce
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;// * transmissionForce
        }
        if (rearWheelDrive)
        {
            //Transmission()
            backLeftWheelCollider.motorTorque = verticalInput * motorForce;// * transmissionForce
            backRightWheelCollider.motorTorque = verticalInput * motorForce;// * transmissionForce
        }

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void Transmission()
    {
        //if (speed >= 0) || (speed < 10)
        //{
        //  transmissionForce = 2.66f;
        //}
        //else if (speed >= 10) || (speed < 20)
        //{
        //  transmissionForce = 1.78f;
        //}
        //else if (speed >= 20) || (speed < 30)
        //{
        //  transmissionForce = 1.3f;
        //}
        //else if (speed >= 30) || (speed < 40)
        //{
        //  transmissionForce = 1f;
        //}
        //else if (speed >= 40) || (speed < 50)
        //{
        //  transmissionForce = 0.7f;
        //}
        //else if (speed <= 50)
        //{
        //  transmissionForce = 0.5f;
        //}
    }

    private void ApplyBraking()
    {
        if (frontHandBrake)
        {
            frontRightWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        }

        if (rearHandBrake)
        {
            backRightWheelCollider.brakeTorque = currentBrakeForce;
            backLeftWheelCollider.brakeTorque = currentBrakeForce;
        }

        //frontRightWheelCollider.brakeTorque = currentBrakeForce;
        //frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        //backRightWheelCollider.brakeTorque = currentBrakeForce;
        //backLeftWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void HandleRotation()
    {
        if (touchingGround == false)
        {
            float pitch;

            float yaw;

            float roll;

            if (isBraking == false)
            {
                pitch = transform.eulerAngles.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
                yaw = transform.eulerAngles.y + horizontalInput * controlYawFactor;
                transform.rotation = Quaternion.Euler(pitch, yaw, transform.eulerAngles.z);
            }
            else if (isBraking)
            {
                pitch = transform.eulerAngles.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
                roll = transform.eulerAngles.z + horizontalInput * -controlRollFactor;
                transform.rotation = Quaternion.Euler(pitch, transform.eulerAngles.y, roll);
            }

        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void ResetCheck()
    {
        if (isResetting == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
