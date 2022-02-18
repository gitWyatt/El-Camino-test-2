using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private float flipCheck;

    private bool isBraking;
    private bool isResetting;
    private bool isFlipping;

    private bool touchingGround;
    private float distToGround = 0.65f;

    double mph;

    [SerializeField] public Text velocityOutput;
    [SerializeField] public Text speedOutput;

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
    [SerializeField] InputAction flip;
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
        flip.Enable();
    }

    private void OnDisable()
    {
        steering.Disable();
        gamepadVertical.Disable();
        handbrake.Disable();
        reset.Disable();
        flip.Disable();
    }

    private void FixedUpdate()
    {
        GetInput();
        CheckGround();
        HandleMotor();
        HandleSteering();
        HandleRotation();
        HandleSpeedometer();
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
        flipCheck = flip.ReadValue<float>();
        if (flipCheck > .5)
        { isFlipping = true; } else { isFlipping = false; }
    }

    void CheckGround()
    {
        //if (Physics.Raycast(frontLeftWheelTransform.position, -frontLeftWheelTransform.up, distToGround) &&
        //    Physics.Raycast(frontRightWheelTransform.position, -frontRightWheelTransform.up, distToGround) &&
        //    Physics.Raycast(backLeftWheelTransform.position, -backLeftWheelTransform.up, distToGround) &&
        //    Physics.Raycast(backRightWheelTransform.position, -backRightWheelTransform.up, distToGround))

        if (Physics.Raycast(frontLeftWheelTransform.position, Vector3.down, distToGround) &&
            Physics.Raycast(frontRightWheelTransform.position, Vector3.down, distToGround) &&
            Physics.Raycast(backLeftWheelTransform.position, Vector3.down, distToGround) &&
            Physics.Raycast(backRightWheelTransform.position, Vector3.down, distToGround))
        {
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
        //output to log
        //Debug.Log(touchingGround);
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

            ////with Euler angles, transform.rotation, breaks when upside down
            //if (isBraking == false)
            //{
            //    pitch = transform.eulerAngles.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    yaw = transform.eulerAngles.y + horizontalInput * controlYawFactor;
            //    transform.rotation = Quaternion.Euler(pitch, yaw, transform.eulerAngles.z);
            //}
            //else if (isBraking)
            //{
            //    pitch = transform.eulerAngles.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    roll = transform.eulerAngles.z + horizontalInput * -controlRollFactor;
            //    transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, roll);
            //    Debug.Log(roll);
            //}

            ////without Euler angles, transform.Rotate() with float angles, breaks when upside down
            //if (isBraking == false)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    yaw = transform.rotation.y + horizontalInput * controlYawFactor;
            //    transform.Rotate(pitch, yaw, transform.rotation.z, Space.Self);
            //}
            //else if (isBraking)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    //roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    transform.Rotate(transform.rotation.x, transform.rotation.y, roll, Space.Self);
            //    Debug.Log(roll);
            //}

            ////without Euler angles, transform.Rotate() with Vector3, still fucking breaks upside down
            //if (isBraking == false)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    yaw = transform.rotation.y + horizontalInput * controlYawFactor;
            //    transform.Rotate(Vector3.right, pitch, Space.World);
            //    transform.Rotate(Vector3.up, yaw, Space.World);
            //}
            //else if (isBraking)
            //{
            //    //pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    transform.Rotate(Vector3.forward, roll, Space.World);
            //    Debug.Log(roll);
            //}

            //same as above but with rigidbody addtorque
            if (isBraking == false)
            {
                pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
                yaw = transform.rotation.y + horizontalInput * controlYawFactor;
                carRigidBody.AddRelativeTorque(Vector3.right * pitch);
                carRigidBody.AddRelativeTorque(Vector3.up * yaw);
            }
            else if (isBraking)
            {
                pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
                roll = transform.rotation.z + horizontalInput * -controlRollFactor;
                carRigidBody.AddRelativeTorque(Vector3.right * pitch);
                carRigidBody.AddRelativeTorque(Vector3.forward * roll);
                Debug.Log(roll);
            }

            //without Euler angles, transform.rotation, breaks when upside down
            //if (isBraking == false)
            //{
            //    //pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    //yaw = transform.rotation.y + horizontalInput * controlYawFactor;
            //    pitch = gamepadVerticalInput;
            //    yaw = horizontalInput;
            //    transform.rotation *= Quaternion.AngleAxis(pitch, Vector3.right);
            //    transform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);
            //}
            //else if (isBraking)
            //{
            //    //pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    //roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    roll = -horizontalInput;
            //    transform.rotation *= Quaternion.AngleAxis(roll, Vector3.forward);
            //    Debug.Log(roll);
            //}

            ////idk dude I found this one in a youtube video, breaks when upside down
            //if (isBraking == false)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    yaw = transform.rotation.y + horizontalInput * controlYawFactor;
            //    transform.rotation *= Quaternion.AngleAxis(pitch, Vector3.right);
            //    transform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);
            //}
            //else if (isBraking)
            //{
            //    //pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    transform.rotation *= Quaternion.AngleAxis(roll, Vector3.forward);
            //    Debug.Log(roll);
            //}

            //split up.  set destination rotation position as new vector3, then transform to it with eulerAngles
            //if (isBraking == false)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    yaw = transform.rotation.y + horizontalInput * controlYawFactor;
            //    Vector3 newRotation = new Vector3(pitch, yaw, transform.rotation.z);
            //    transform.Rotate(newRotation);
            //}
            //else if (isBraking)
            //{
            //    pitch = transform.rotation.x + gamepadVerticalInput * controlPitchFactor; //specific to gamepad for time being, needs fixing
            //    //roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    roll = transform.rotation.z + horizontalInput * -controlRollFactor;
            //    Vector3 newRotation = new Vector3(transform.rotation.x, transform.rotation.y, roll);
            //    transform.Rotate(newRotation);
            //    Debug.Log(roll);
            //}

            float help = transform.rotation.z;
                Debug.Log(help);
        }
    }

    private void HandleSpeedometer()        //need to correct for up/down.  speed ramps up when freefalling, etc.
    {
        mph = carRigidBody.velocity.magnitude * 2.237;      //calculate accurate mph
        //kph = carRigidBody.velocity.magnitude * 3.6;      //calculate accurate kph
        velocityOutput.text = carRigidBody.velocity.magnitude.ToString("F2") + "Units";
        speedOutput.text = mph.ToString("F2") + "MPH";
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
        if (isFlipping == true)
        {
            carRigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        }
    }
}
