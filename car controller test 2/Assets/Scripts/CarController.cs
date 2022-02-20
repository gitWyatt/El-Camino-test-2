using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    [SerializeField] private InputMaster controls;

    public Transform centerOfMass;

    private Rigidbody carRigidBody;

    //I don't think I need this, might be leftover from old input system
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float transmissionForce;

    private float horizontalInput;
    private float verticalInput;
    private float gas;
    private float gamepadVerticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private float brakeCheck;
    private float resetCheck;
    private float flipCheck;
    private float pauseCheck;

    private bool isBraking;
    private bool isResetting;
    private bool isFlipping;
    public bool pressingPause;

    private bool touchingGround;
    private float distToGround = 0.65f;

    double mph;
    private float engineRPM;
    private float minRPM = 1000f;
    private float maxRPM = 2500f;
    private float currentGear = 1f;
    private float gearNumber = 6f;
    private float[] gearRatio = new float[] { 2.66f, 1.78f, 1.3f, 1f, .7f, .5f };


    [SerializeField] public Text rpmOutput;
    [SerializeField] public Text velocityOutput;
    [SerializeField] public Text speedOutput;
    [SerializeField] public Text gearBox;

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

    //[SerializeField] InputAction steering;
    //[SerializeField] InputAction gamepadVertical;
    //[SerializeField] InputAction handbrake;
    //[SerializeField] InputAction flip;
    //[SerializeField] InputAction reset;
    //[SerializeField] InputAction pause;

    private void Awake()
    {
        controls = new InputMaster();
    }

    private void Start()
    {
        //sets center of mass to transform
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = centerOfMass.localPosition;
    }

    private void OnEnable()
    {
        controls.Enable();
        //steering.Enable();
        //gamepadVertical.Enable();
        //handbrake.Enable();
        //reset.Enable();
        //flip.Enable();
        //pause.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
        //steering.Disable();
        //gamepadVertical.Disable();
        //handbrake.Disable();
        //reset.Disable();
        //flip.Disable();
        //pause.Disable();
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
        //new input

        horizontalInput = controls.Player.Steering.ReadValue<Vector2>().x;
        verticalInput = controls.Player.UpDown.ReadValue<Vector2>().y;
        gas = controls.Player.ForwardReverse.ReadValue<Vector2>().x;

        if (controls.Player.Handbrake.triggered)
        {
            isBraking = true;
            Debug.Log(isBraking);
        }

        if (controls.Player.Reset.triggered)
        {
            isResetting = true;
        }

        if (controls.Player.Flip.triggered)
        {
            isFlipping = true;
        }

        if (controls.Player.Pause.triggered)
        {
            pressingPause = true;
        }
        
        //old input

        //horizontalInput = Input.GetAxis(HORIZONTAL);
        //verticalInput = Input.GetAxis(VERTICAL);
        //isBraking = Input.GetKey(KeyCode.Space);
        //idk maybe

        //horizontalInput = steering.ReadValue<Vector2>().x;
        //verticalInput = steering.ReadValue<Vector2>().y;
        //gamepadVerticalInput = gamepadVertical.ReadValue<float>();

        //brakeCheck = handbrake.ReadValue<float>();
        //if (brakeCheck > .5)
        //{ isBraking = true; } else { isBraking = false; }
        
        //resetCheck = reset.ReadValue<float>();
        //if (resetCheck > .5)
        //{ isResetting = true; } else { isResetting = false; }
        
        //flipCheck = flip.ReadValue<float>();
        //if (flipCheck > .5)
        //{ isFlipping = true; } else { isFlipping = false; }

        //pauseCheck = pause.ReadValue<float>();
        //if (pauseCheck > .5)
        //{ pressingPause = true; } else { pressingPause = false; }
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
            Transmission();
            frontLeftWheelCollider.motorTorque = gas * motorForce * transmissionForce;
            frontRightWheelCollider.motorTorque = gas * motorForce * transmissionForce;
        }
        if (rearWheelDrive)
        {
            Transmission();
            backLeftWheelCollider.motorTorque = gas * motorForce * transmissionForce;
            backRightWheelCollider.motorTorque = gas * motorForce * transmissionForce;
        }

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void Transmission()
    {
        if (frontWheelDrive)
        {
            engineRPM = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f * transmissionForce;
        }
        else
        {
            engineRPM = (backLeftWheelCollider.rpm + backRightWheelCollider.rpm) / 2f * transmissionForce;
        }

        if (engineRPM >= maxRPM)
        {
            if (currentGear < gearNumber)
            {
                currentGear++;

                switch(currentGear)
                {
                    case 2:
                        transmissionForce = 1.78f;
                        break;
                    case 3:
                        transmissionForce = 1.3f;
                        break;
                    case 4:
                        transmissionForce = 1f;
                        break;
                    case 5:
                        transmissionForce = .7f;
                        break;
                    case 6:
                        transmissionForce = .5f;
                        break;
                }
            }
            else
            {
                currentGear = 6;
                transmissionForce = .5f;
            }
        }
        if (engineRPM <= minRPM)
        {
            if (currentGear > 1)
            {
                currentGear--;

                switch (currentGear)
                {
                    case 1:
                        transmissionForce = 2.66f;
                        break;
                    case 2:
                        transmissionForce = 1.78f;
                        break;
                    case 3:
                        transmissionForce = 1.3f;
                        break;
                    case 4:
                        transmissionForce = 1f;
                        break;
                    case 5:
                        transmissionForce = .7f;
                        break;
                }
            }
            else
            {
                currentGear = 1;
                transmissionForce = 2.66f;
            }
        }

        //Debug.Log(transmissionForce);
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
                //Debug.Log(roll);
            }
            //float help = transform.rotation.z;
            //Debug.Log(help);
        }
    }

    private void HandleSpeedometer()
    {
        Vector3 carVelocity = carRigidBody.velocity;
        carVelocity.y = 0;                          //ignore inaccuracies due to y axis
        mph = carVelocity.magnitude * 2.237;        //calculate accurate mph
        //kph = carRigidBody.velocity.magnitude * 3.6;      //calculate accurate kph
        gearBox.text = "Gear: " + currentGear.ToString();
        rpmOutput.text = "eRPM: " + engineRPM.ToString("F0");
        velocityOutput.text = "Units: " + carVelocity.magnitude.ToString("F0");
        speedOutput.text = "MPH: " + mph.ToString("F0");
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



//~~~~~~~~~~~Old code graveyard~~~~~~~~~~~//

//HandleRotation Class

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