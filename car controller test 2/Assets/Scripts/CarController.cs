using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public InputMaster controls;

    public Transform centerOfMassAir;
    public Transform centerOfMassGround;
    public Transform centerOfMassCurrent;

    private Rigidbody carRigidBody;

    public PauseMenu pauseMenu;

    //I don't think I need this, might be leftover from old input system
    //private const string HORIZONTAL = "Horizontal";
    //private const string VERTICAL = "Vertical";

    private float transmissionForce;

    private float horizontalInput;
    private float verticalInput;
    private float gas;
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
    private float engineRPM;
    private float minRPM = 1000f;
    private float maxRPM = 2500f;
    private float currentGear = 1f;
    private float gearNumber = 6f;
    private float[] gearRatio = new float[] { 2.66f, 1.78f, 1.3f, 1f, .7f, .1f };  //unneeded as of now.  top gear used to be .5f instead of .1f


    [SerializeField] public Text rpmOutput;
    [SerializeField] public Text velocityOutput;
    [SerializeField] public Text speedOutput;
    [SerializeField] public Text gearBox;

    [SerializeField] public bool frontWheelDrive;
    [SerializeField] public bool rearWheelDrive;
    [SerializeField] private float singleAxleMFAdjustment;
    [SerializeField] private bool frontHandBrake;
    [SerializeField] private bool rearHandBrake;

    [SerializeField] private float motorForce;
    [SerializeField] public int motorSelection;
    [SerializeField] private float streetMotorForce;
    [SerializeField] private float racingMotorForce;

    [SerializeField] private float brakeForce;
    [SerializeField] private float driftBrakesLerpValue;
    [SerializeField] public int tireSelection;
    [SerializeField] private float basicBrakesStandardSidewaysStiffness;
    [SerializeField] private float basicBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float basicBrakesDriftRearSidewaysStiffness;
    [SerializeField] private float offRoadBrakesStandardSidewaysStiffness;
    [SerializeField] private float offRoadBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float offRoadBrakesDriftRearSidewaysStiffness;
    [SerializeField] private float driftBrakesStandardSidewaysStiffness;
    [SerializeField] private float driftBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float driftBrakesDriftRearSidewaysStiffness;

    [SerializeField] private float standardSidewaysStiffness;
    [SerializeField] private float driftFrontSidewaysStiffness;
    [SerializeField] private float driftRearSidewaysStiffness;

    [SerializeField] private float maxSteerAngle;
    [SerializeField] public int steeringSelection;
    [SerializeField] private float steeringLerpValue;
    [SerializeField] private float unpoweredSteeringLerpValue;
    [SerializeField] private float streetSteeringLerpValue;
    [SerializeField] private float driftSteeringLerpValue;

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

    private void Awake()
    {
        if (controls == null)
        {
            controls = new InputMaster();
        }
        Time.timeScale = 1f;
    }

    private void Start()
    {
        //sets center of mass to transform
        carRigidBody = GetComponent<Rigidbody>();
        //carRigidBody.centerOfMass = centerOfMassGround.localPosition;
        carRigidBody.centerOfMass = centerOfMassCurrent.localPosition;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
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
        DebugToConsole();
    }

    private void GetInput()
    {
        //new input player

        horizontalInput = controls.Player.Steering.ReadValue<float>();
        verticalInput = controls.Player.UpDown.ReadValue<float>();
        gas = controls.Player.ForwardReverse.ReadValue<float>();

        brakeCheck = controls.Player.Handbrake.ReadValue<float>();
        resetCheck = controls.Player.Reset.ReadValue<float>();
        flipCheck = controls.Player.Flip.ReadValue<float>();
        //pauseCheck = controls.Player.Pause.ReadValue<float>();
        controls.Player.Pause.performed += PauseHandler;

        if (brakeCheck > .5)
        { isBraking = true; }
        else { isBraking = false; }

        if (resetCheck > .5)
        { isResetting = true; }
        else { isResetting = false; }

        if (flipCheck > .5)
        { isFlipping = true; }
        else { isFlipping = false; }
    }

    void CheckGround()
    {
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
        //Debug.Log(touchingGround);
    }

    private void HandleMotor()
    {
        switch (motorSelection)
        {
            case 0:
                motorForce = streetMotorForce;
                break;
            case 1:
                motorForce = racingMotorForce;
                break;
        }
        

        if (frontWheelDrive && rearWheelDrive)
        {
            Transmission();
            frontLeftWheelCollider.motorTorque = gas * motorForce * transmissionForce;
            frontRightWheelCollider.motorTorque = gas * motorForce * transmissionForce;
            backLeftWheelCollider.motorTorque = gas * motorForce * transmissionForce;
            backRightWheelCollider.motorTorque = gas * motorForce * transmissionForce;
        }
        else if (frontWheelDrive && !rearWheelDrive)
        {
            Transmission();
            frontLeftWheelCollider.motorTorque = gas * (motorForce * singleAxleMFAdjustment) * transmissionForce;
            frontRightWheelCollider.motorTorque = gas * (motorForce * singleAxleMFAdjustment) * transmissionForce;
            backLeftWheelCollider.motorTorque = 0f;
            backRightWheelCollider.motorTorque = 0f;
        }
        else if (rearWheelDrive && !frontWheelDrive)
        {
            Transmission();
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
            backLeftWheelCollider.motorTorque = gas * (motorForce * singleAxleMFAdjustment) * transmissionForce;
            backRightWheelCollider.motorTorque = gas * (motorForce * singleAxleMFAdjustment) * transmissionForce;
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
                        transmissionForce = .1f;    //was previously .5f
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
            if (engineRPM < 0)
            {
                transmissionForce = 1.78f;
            }
            else
            {
                currentGear = 1;
                transmissionForce = 2.66f;
            }
        }

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

        WheelFrictionCurve frontLeftWheelFriction = frontLeftWheelCollider.sidewaysFriction;
        WheelFrictionCurve frontRightWheelFriction = frontRightWheelCollider.sidewaysFriction;
        WheelFrictionCurve backLeftWheelFriction = backLeftWheelCollider.sidewaysFriction;
        WheelFrictionCurve backRightWheelFriction = backRightWheelCollider.sidewaysFriction;

        switch (tireSelection)
        {
            case 0:
                standardSidewaysStiffness = basicBrakesStandardSidewaysStiffness;
                driftFrontSidewaysStiffness = basicBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = basicBrakesDriftRearSidewaysStiffness;
                break;
            case 1:
                standardSidewaysStiffness = offRoadBrakesStandardSidewaysStiffness;
                driftFrontSidewaysStiffness = offRoadBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = offRoadBrakesDriftRearSidewaysStiffness;
                break;
            case 2:
                standardSidewaysStiffness = driftBrakesStandardSidewaysStiffness;
                driftFrontSidewaysStiffness = driftBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = driftBrakesDriftRearSidewaysStiffness;
                break;
        }

        float currentDriftFrontSidewaysStiffness = frontLeftWheelFriction.stiffness;
        float currentDriftRearSidewaysStiffness = frontLeftWheelFriction.stiffness;

        if (isBraking)
        {
            //lerp
            //frontLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, driftFrontSidewaysStiffness, driftBrakesLerpValue);
            //frontRightWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, driftFrontSidewaysStiffness, driftBrakesLerpValue);
            //backLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, driftRearSidewaysStiffness, driftBrakesLerpValue);
            //backRightWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, driftRearSidewaysStiffness, driftBrakesLerpValue);

            //non-lerp
            frontLeftWheelFriction.stiffness = driftFrontSidewaysStiffness;
            frontRightWheelFriction.stiffness = driftFrontSidewaysStiffness;
            backLeftWheelFriction.stiffness = driftRearSidewaysStiffness;
            backRightWheelFriction.stiffness = driftRearSidewaysStiffness;

            frontLeftWheelCollider.sidewaysFriction = frontLeftWheelFriction;
            frontRightWheelCollider.sidewaysFriction = frontRightWheelFriction;
            backLeftWheelCollider.sidewaysFriction = backLeftWheelFriction;
            backRightWheelCollider.sidewaysFriction = backRightWheelFriction;

            Debug.Log(frontLeftWheelFriction.stiffness);
        }
        else if (!isBraking)
        {
            //lerp
            frontLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
            frontRightWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
            backLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
            backRightWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);

            //non-lerp
            //frontLeftWheelFriction.stiffness = standardSidewaysStiffness;
            //frontRightWheelFriction.stiffness = standardSidewaysStiffness;
            //backLeftWheelFriction.stiffness = standardSidewaysStiffness;
            //backRightWheelFriction.stiffness = standardSidewaysStiffness;

            frontLeftWheelCollider.sidewaysFriction = frontLeftWheelFriction;
            frontRightWheelCollider.sidewaysFriction = frontRightWheelFriction;
            backLeftWheelCollider.sidewaysFriction = backLeftWheelFriction;
            backRightWheelCollider.sidewaysFriction = backRightWheelFriction;

            Debug.Log(frontLeftWheelFriction.stiffness);
        }
    }

    private void HandleSteering()
    {
        currentSteerAngle = (frontLeftWheelCollider.steerAngle + frontRightWheelCollider.steerAngle) / 2;
        float newSteerAngle = maxSteerAngle * horizontalInput;

        switch (steeringSelection)
        {
            case 0:
                steeringLerpValue = .1f;
                brakeForce = 400f;          //move this to its own dropdown, maybe "handbrake orientation => standard handbrake"
                break;
            case 1:
                steeringLerpValue = .3f;
                brakeForce = 400f;          //move this to its own dropdown, maybe "handbrake orientation => standard handbrake"
                break;
            case 2:
                steeringLerpValue = .5f;
                brakeForce = 200f;          //move this to its own dropdown, maybe "handbrake orientation => drifting handbrake"
                break;
        }
 
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(currentSteerAngle, newSteerAngle, steeringLerpValue);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(currentSteerAngle, newSteerAngle, steeringLerpValue);

        //currentSteerAngle = maxSteerAngle * horizontalInput;
        //frontLeftWheelCollider.steerAngle = currentSteerAngle;
        //frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void HandleRotation()
    {
        if (touchingGround == false)
        {
            centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassAir.localPosition, 0.1f);
            //carRigidBody.centerOfMass = centerOfMassAir.localPosition;

            float pitch;
            float yaw;
            float roll;

            if (isBraking == false)
            {
                pitch = transform.rotation.x + verticalInput * controlPitchFactor;
                if (gas < 0)
                {
                    yaw = transform.rotation.y + -horizontalInput * controlYawFactor;
                }
                else
                {
                    yaw = transform.rotation.y + horizontalInput * controlYawFactor;
                }
                //yaw = transform.rotation.y + horizontalInput * controlYawFactor;
                carRigidBody.AddRelativeTorque(Vector3.right * pitch);
                carRigidBody.AddRelativeTorque(Vector3.up * yaw);
            }
            else if (isBraking)
            {
                pitch = transform.rotation.x + verticalInput * controlPitchFactor; 
                roll = transform.rotation.z + horizontalInput * -controlRollFactor;
                carRigidBody.AddRelativeTorque(Vector3.right * pitch);
                carRigidBody.AddRelativeTorque(Vector3.forward * roll);
            }
        }

        //big test right here
        if (touchingGround)
        {
            centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassGround.localPosition, 0.5f);
            //carRigidBody.centerOfMass = centerOfMassGround.localPosition;
        }

        Debug.Log(centerOfMassCurrent.localPosition);
    }

    public void PauseHandler(InputAction.CallbackContext context)
    {
        pauseMenu.OnPause();

        if (Time.timeScale > 0.9f)
        {
            //verticalInput = controls.Menu.UpDown.ReadValue<float>();
        }
        else
        {
            //verticalInput = controls.Player.ForwardReverse.ReadValue<float>();
        }


        //FindObjectOfType<PauseMenu>().OnPause();
        //Debug.Log("step 1");
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

    private void DebugToConsole()
    {
        //Debug.Log();    
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



//new input menu
//horizontalPause = controls.Menu.LeftRight.ReadValue<float>();
//verticalPause = controls.Menu.UpDown.ReadValue<float>();
//controls.Menu.Pause.performed += PauseHandler;

//if (pauseCheck > .5)
//{ pressingPause = true; }
//else { pressingPause = false; }

//if (controls.Player.Handbrake.triggered)
//{
//    isBraking = true;
//}

//if (controls.Player.Reset.triggered)
//{
//    isResetting = true;
//    Debug.Log(isResetting);
//}

//if (controls.Player.Flip.triggered)
//{
//    isFlipping = true;
//}

//if (controls.Player.Pause.triggered)
//{
//    pressingPause = true;
//}

//horizontalInput = Input.GetAxis(HORIZONTAL);
//verticalInput = Input.GetAxis(VERTICAL);
//isBraking = Input.GetKey(KeyCode.Space);

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