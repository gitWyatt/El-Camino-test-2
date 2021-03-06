using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;

public class CarController : MonoBehaviour
{
    CarEffects carEffects;

    public InputMaster controls;

    public Transform centerOfMassCenterAir;
    public Transform centerOfMassCenterGround;
    public Transform centerOfMassFrontAir;
    public Transform centerOfMassFrontGround;
    public Transform centerOfMassRearAir;
    public Transform centerOfMassRearGround;
    public Transform centerOfMassCurrent;

    private Rigidbody carRigidBody;

    public PauseMenu pauseMenu;

    private float transmissionForce;

    private float horizontalInput;
    private float verticalInput;
    private float gas;
    private float cameraInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private float brakeCheck;
    private float resetCheck;
    private float flipCheck;
    private float engageCheck;

    public bool isCamming;
    public bool isBraking;
    public bool isReversing;
    private bool isResetting;
    private bool isFlipping;
    private bool isEngaging;

    public bool touchingGround;
    //private float distToGround = 0.505f;

    double mph;
    private float engineRPM;
    [SerializeField] private float minRPM;
    [SerializeField] private float maxRPM;
    private float currentGear = 1f;
    private float gearNumber = 6f;
    private float[] gearRatio = new float[] { 2.66f, 1.78f, 1.3f, 1f, .7f, .1f };  //unneeded as of now.  top gear used to be .5f instead of .1f
    private float steerAngle;

    [SerializeField] public int bodySelection;

    [SerializeField] public Text rpmOutput;
    [SerializeField] public Text velocityOutput;
    [SerializeField] public Text speedOutput;
    [SerializeField] public Text gearBox;
    [SerializeField] public Image boostMeter;
    [SerializeField] public CanvasGroup boostCanvasGroup;

    [SerializeField] private float flightGravity;
    [SerializeField] private float flightUpwardsForce;
    [SerializeField] float upwardsForce;
    [SerializeField] float upwardsPull;

    [SerializeField] public int passiveSelection;
    [SerializeField] public int useButtonSelection;
    [SerializeField] private float jumpForce;
    [SerializeField] private float thrusterForce;
    [SerializeField] private float groundBoostForce;
    [SerializeField] public bool airBoosting = false;
    [SerializeField] public bool groundBoosting = false;

    public float playerAirThrusterFuel = 100f;
    public bool airThrusterFlag = false;
    [SerializeField] private float maxAirThrusterFuel = 100f;
    [SerializeField] private float airThrusterDrain = 0.5f;
    [SerializeField] private float airThrusterRegen = 0.5f;
    [SerializeField] private float airThrusterCutoff = 40f;
    public float playerGroundBoostFuel = 100f;
    public bool groundBoostFlag = false;
    [SerializeField] private float maxGroundBoostFuel = 100f;
    [SerializeField] private float groundBoostDrain = 0.5f;
    [SerializeField] private float groundBoostRegen = 0.5f;
    [SerializeField] private float groundBoostCutoff = 70f;

    [SerializeField] public bool frontWheelDrive;
    [SerializeField] public bool rearWheelDrive;
    [SerializeField] private float singleAxleMFAdjustment;
    [SerializeField] private bool frontHandBrake;
    [SerializeField] private bool rearHandBrake;

    [SerializeField] private float motorForce;
    [SerializeField] public int motorSelection;
    [SerializeField] private float streetMotorForce;
    [SerializeField] private float racingMotorForce;

    [SerializeField] public int transmissionSelection;

    [SerializeField] private float brakeForce;
    [SerializeField] private float driftBrakesLerpValue;
    [SerializeField] public int handbrakeSelection;
    [SerializeField] private float moreDriftBrakeForce;
    [SerializeField] private float lessDriftBrakeForce;
    [SerializeField] private float driftForwardStiffnessConstant;
    [SerializeField] private float basicBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float basicBrakesDriftRearSidewaysStiffness;
    [SerializeField] private float offRoadBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float offRoadBrakesDriftRearSidewaysStiffness;
    [SerializeField] private float driftBrakesDriftFrontSidewaysStiffness;
    [SerializeField] private float driftBrakesDriftRearSidewaysStiffness;

    [SerializeField] public int tireSelection;
    [SerializeField] private float basicBrakesStandardSidewaysStiffness;
    [SerializeField] private float offRoadBrakesStandardSidewaysStiffness;
    [SerializeField] private float driftBrakesStandardSidewaysStiffness;

    [SerializeField] private float standardSidewaysStiffness;
    [SerializeField] private float driftFrontSidewaysStiffness;
    [SerializeField] private float driftRearSidewaysStiffness;

    [SerializeField] private float maxSteerAngle;
    [SerializeField] public int steeringAngleSelection;
    [SerializeField] private float streetSteerAngle;
    [SerializeField] private float driftSteerAngle;
    [SerializeField] public int steeringPowerSelection;
    [SerializeField] private float steeringLerpValue;
    [SerializeField] private float unpoweredSteeringLerpValue;
    [SerializeField] private float poweredSteeringLerpValue;

    [SerializeField] private float suspensionPower;
    [SerializeField] public int suspensionSelection;
    [SerializeField] private float bouncySuspensionValue;
    [SerializeField] private float middleSuspensionValue;
    [SerializeField] private float sturdySuspensionValue;
    [SerializeField] private float rockSuspensionValue;

    [SerializeField] private float controlPitchFactor;
    [SerializeField] private float controlYawFactor;
    [SerializeField] private float controlRollFactor;

    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider backLeftWheelCollider;
    [SerializeField] public WheelCollider backRightWheelCollider;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    [SerializeField] public GameObject regularWheelGroup;
    [SerializeField] public WheelCollider frontLeftRegularWheelCollider;
    [SerializeField] public WheelCollider frontRightRegularWheelCollider;
    [SerializeField] public WheelCollider backLeftRegularWheelCollider;
    [SerializeField] public WheelCollider backRightRegularWheelCollider;
    [SerializeField] private Transform frontLeftRegularWheelTransform;
    [SerializeField] private Transform frontRightRegularWheelTransform;
    [SerializeField] private Transform backLeftRegularWheelTransform;
    [SerializeField] private Transform backRightRegularWheelTransform;

    [SerializeField] public GameObject bigWheelGroup;
    [SerializeField] public WheelCollider frontLeftBigWheelCollider;
    [SerializeField] public WheelCollider frontRightBigWheelCollider;
    [SerializeField] public WheelCollider backLeftBigWheelCollider;
    [SerializeField] public WheelCollider backRightBigWheelCollider;
    [SerializeField] private Transform frontLeftBigWheelTransform;
    [SerializeField] private Transform frontRightBigWheelTransform;
    [SerializeField] private Transform backLeftBigWheelTransform;
    [SerializeField] private Transform backRightBigWheelTransform;

    [SerializeField] CinemachineVirtualCamera CMCamera;
    CinemachineTransposer cameraTransposer;

    Color thatPurpleyColor;

    private void Awake()
    {
        if (controls == null)
        {
            controls = new InputMaster();
        }
        Time.timeScale = 1f;
        carEffects = GameObject.Find("Camino").GetComponent<CarEffects>();
        thatPurpleyColor = boostMeter.color;
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
        HandleThruster();   //sloppy, but its because of the new input system
        HandleSuspension();
        HandleHandbrake();
        HandleMotor();
        HandleSteering();
        HandleRotation();
        HandlePassive();
        HandleSpeedometer();
        HandleCamera();
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

        cameraInput = controls.Player.Camera.ReadValue<float>();

        brakeCheck = controls.Player.Handbrake.ReadValue<float>();
        resetCheck = controls.Player.Reset.ReadValue<float>();
        flipCheck = controls.Player.Flip.ReadValue<float>();
        controls.Player.Engage.performed += EngageHandler;
        engageCheck = controls.Player.Engage.ReadValue<float>();
        controls.Player.Pause.performed += PauseHandler;

        if (brakeCheck > .5)
        { isBraking = true; }
        else { isBraking = false; }

        if (gas < 0)
        { isReversing = true; }
        else { isReversing = false; }

        if (resetCheck > .5)
        { isResetting = true; }
        else { isResetting = false; }

        if (flipCheck > .5)
        { isFlipping = true; }
        else { isFlipping = false; }

        if (engageCheck > .5)
        { isEngaging = true; }
        else { isEngaging = false; }

        if (Mathf.Abs(cameraInput) > .1f)
        { isCamming = true; }
        else { isCamming = false; }
    }

    void CheckGround()
    {
        //float distToGround = frontLeftWheelCollider.bounds.extents.y;
        //if (Physics.Raycast(frontLeftWheelTransform.position, -Vector3.up, distToGround) &&
        //    Physics.Raycast(frontRightWheelTransform.position, -Vector3.up, distToGround) &&
        //    Physics.Raycast(backLeftWheelTransform.position, -Vector3.up, distToGround) &&
        //    Physics.Raycast(backRightWheelTransform.position, -Vector3.up, distToGround))
        //{
        //    touchingGround = true;
        //}
        //else
        //{
        //    touchingGround = false;
        //}
        //Debug.Log(touchingGround);

        if (frontLeftWheelCollider.isGrounded && backLeftWheelCollider.isGrounded && frontRightWheelCollider.isGrounded && backRightWheelCollider.isGrounded)
        {
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    private void HandleSuspension()
    {
        var flSpring = frontLeftWheelCollider.suspensionSpring;
        var frSpring = frontRightWheelCollider.suspensionSpring;
        var blSpring = backLeftWheelCollider.suspensionSpring;
        var brSpring = backRightWheelCollider.suspensionSpring;

        switch (suspensionSelection)
        {
            case 0:
                flSpring.damper = bouncySuspensionValue;
                frSpring.damper = bouncySuspensionValue;
                blSpring.damper = bouncySuspensionValue;
                brSpring.damper = bouncySuspensionValue;
                break;
            case 1:
                flSpring.damper = middleSuspensionValue;
                frSpring.damper = middleSuspensionValue;
                blSpring.damper = middleSuspensionValue;
                brSpring.damper = middleSuspensionValue;
                break;
            case 2:
                flSpring.damper = sturdySuspensionValue;
                frSpring.damper = sturdySuspensionValue;
                blSpring.damper = sturdySuspensionValue;
                brSpring.damper = sturdySuspensionValue;
                break;
            case 3:
                flSpring.damper = rockSuspensionValue;
                frSpring.damper = rockSuspensionValue;
                blSpring.damper = rockSuspensionValue;
                brSpring.damper = rockSuspensionValue;
                break;
        }

        frontLeftWheelCollider.suspensionSpring = flSpring;
        frontRightWheelCollider.suspensionSpring = frSpring;
        backLeftWheelCollider.suspensionSpring = blSpring;
        backRightWheelCollider.suspensionSpring = frSpring;
    }

    private void HandleHandbrake()
    {
        switch (handbrakeSelection)
        {
            case 0:
                frontHandBrake = false;
                brakeForce = moreDriftBrakeForce;
                driftFrontSidewaysStiffness = driftBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = driftBrakesDriftRearSidewaysStiffness;
                break;
            case 1:
                frontHandBrake = false;
                brakeForce = lessDriftBrakeForce;
                driftFrontSidewaysStiffness = basicBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = basicBrakesDriftRearSidewaysStiffness;
                break;
            case 2:
                frontHandBrake = true;
                if (frontWheelDrive && rearWheelDrive)
                {
                    brakeForce = motorForce * singleAxleMFAdjustment;
                }
                else if (!frontWheelDrive || !rearWheelDrive)
                {
                    brakeForce = motorForce;
                }
                driftFrontSidewaysStiffness = offRoadBrakesDriftFrontSidewaysStiffness;
                driftRearSidewaysStiffness = offRoadBrakesDriftRearSidewaysStiffness;
                break;
        }
        //Debug.Log(brakeForce);
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
            case 2:
                motorForce = 500f;
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

        //4 SPEED TRANSMISSION
        if (transmissionSelection == 0)
        {
            if (engineRPM >= maxRPM)
            {
                if (currentGear < gearNumber)
                {
                    currentGear++;

                    switch (currentGear)
                    {
                        case 2:
                            transmissionForce = 2f;
                            break;
                        case 3:
                            transmissionForce = 1f;
                            break;
                        case 4:
                            transmissionForce = .01f;
                            break;
                    }
                }
                else
                {
                    currentGear = 4;
                    transmissionForce = .01f;
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
                            transmissionForce = 2.5f;
                            break;
                        case 2:
                            transmissionForce = 2;
                            break;
                        case 3:
                            transmissionForce = 1f;
                            break;
                    }
                }
                if (engineRPM < 0)
                {
                    transmissionForce = 2.5f;
                }
                else
                {
                    currentGear = 1;
                    transmissionForce = 2.5f;
                }
            }
        }

        //6 SPEED TRANSMISSION
        if (transmissionSelection == 1)
        {
            if (engineRPM >= maxRPM)
            {
                if (currentGear < gearNumber)
                {
                    currentGear++;

                    switch (currentGear)
                    {
                        case 2:
                            transmissionForce = 2f;
                            break;
                        case 3:
                            transmissionForce = 1.5f;
                            break;
                        case 4:
                            transmissionForce = 1f;
                            break;
                        case 5:
                            transmissionForce = .5f;
                            break;
                        case 6:
                            transmissionForce = .01f;
                            break;
                    }
                }
                else
                {
                    currentGear = 6;
                    transmissionForce = .01f;
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
                            transmissionForce = 2.5f;
                            break;
                        case 2:
                            transmissionForce = 2f;
                            break;
                        case 3:
                            transmissionForce = 1.5f;
                            break;
                        case 4:
                            transmissionForce = 1f;
                            break;
                        case 5:
                            transmissionForce = .5f;
                            break;
                    }
                }
                if (engineRPM < 0)
                {
                    transmissionForce = 2.5f;
                }
                else
                {
                    currentGear = 1;
                    transmissionForce = 2.5f;
                }
            }
        }
        //6 SPEED SPORT
        if (transmissionSelection == 2)
        {
            if (engineRPM >= maxRPM)
            {
                if (currentGear < gearNumber)
                {
                    currentGear++;

                    switch (currentGear)
                    {
                        case 2:
                            transmissionForce = 3f;
                            break;
                        case 3:
                            transmissionForce = 2f;
                            break;
                        case 4:
                            transmissionForce = 1.3f;
                            break;
                        case 5:
                            transmissionForce = .7f;
                            break;
                        case 6:
                            transmissionForce = .01f;
                            break;
                    }
                }
                else
                {
                    currentGear = 6;
                    transmissionForce = .01f;
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
                            transmissionForce = 3.5f;
                            break;
                        case 2:
                            transmissionForce = 3f;
                            break;
                        case 3:
                            transmissionForce = 2f;
                            break;
                        case 4:
                            transmissionForce = 1.3f;
                            break;
                        case 5:
                            transmissionForce = .7f;
                            break;
                    }
                }
                if (engineRPM < 0)
                {
                    transmissionForce = 3.5f;
                }
                else
                {
                    currentGear = 1;
                    transmissionForce = 3.5f;
                }
            }
        }

        //6 SPEED DRIFT
        if (transmissionSelection == 3)
        {
            if (engineRPM >= maxRPM)
            {
                if (currentGear < gearNumber)
                {
                    currentGear++;

                    switch (currentGear)
                    {
                        case 2:
                            transmissionForce = 2f;
                            break;
                        case 3:
                            transmissionForce = 1f;
                            break;
                        case 4:
                            transmissionForce = .5f;
                            break;
                        case 5:
                            transmissionForce = .1f;
                            break;
                        case 6:
                            transmissionForce = .00001f;
                            break;
                    }
                }
                else
                {
                    currentGear = 6;
                    transmissionForce = .00001f;
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
                            transmissionForce = 4f;
                            break;
                        case 2:
                            transmissionForce = 2f;
                            break;
                        case 3:
                            transmissionForce = 1f;
                            break;
                        case 4:
                            transmissionForce = .5f;
                            break;
                        case 5:
                            transmissionForce = .1f;
                            break;
                    }
                }
                if (engineRPM < 0)
                {
                    transmissionForce = 3.5f;
                }
                else
                {
                    currentGear = 1;
                    transmissionForce = 3.5f;
                }
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

        WheelFrictionCurve frontLeftWheelSidewaysFriction = frontLeftWheelCollider.sidewaysFriction;
        WheelFrictionCurve frontRightWheelSidewaysFriction = frontRightWheelCollider.sidewaysFriction;
        WheelFrictionCurve backLeftWheelSidewaysFriction = backLeftWheelCollider.sidewaysFriction;
        WheelFrictionCurve backRightWheelSidewaysFriction = backRightWheelCollider.sidewaysFriction;

        WheelFrictionCurve frontLeftWheelForwardFriction = frontLeftWheelCollider.forwardFriction;
        WheelFrictionCurve frontRightWheelForwardFriction = frontRightWheelCollider.forwardFriction;
        WheelFrictionCurve backLeftWheelForwardFriction = backLeftWheelCollider.forwardFriction;
        WheelFrictionCurve backRightWheelForwardFriction = backRightWheelCollider.forwardFriction;

        switch (tireSelection)
        {
            case 0:
                standardSidewaysStiffness = basicBrakesStandardSidewaysStiffness;
                break;
            case 1:
                standardSidewaysStiffness = offRoadBrakesStandardSidewaysStiffness;
                break;
            case 2:
                standardSidewaysStiffness = driftBrakesStandardSidewaysStiffness;
                break;
        }

        if (Vector3.Distance(transform.forward, Vector3.Normalize(carRigidBody.velocity)) < .2f)
        {
            standardSidewaysStiffness = standardSidewaysStiffness * 2.5f;
        }

        if (Vector3.Distance(transform.forward, Vector3.Normalize(carRigidBody.velocity)) > .35f)
        {
            standardSidewaysStiffness = standardSidewaysStiffness * .9f;
            //Debug.Log("ALSDKFJLASDKJFAJSD");
        }
        else
        {
            //Debug.Log("farts lol");
        }


        float currentDriftFrontSidewaysStiffness = (frontLeftWheelSidewaysFriction.stiffness + frontRightWheelSidewaysFriction.stiffness) / 2;
        float currentDriftRearSidewaysStiffness = (backLeftWheelSidewaysFriction.stiffness + backRightWheelSidewaysFriction.stiffness) / 2;

        if (isBraking)
        {
            //forward friction adjustment
            if (frontWheelDrive && !rearWheelDrive)
            {
                frontLeftWheelForwardFriction.stiffness = frontLeftWheelForwardFriction.stiffness * driftForwardStiffnessConstant;
                frontRightWheelForwardFriction.stiffness = frontRightWheelForwardFriction.stiffness * driftForwardStiffnessConstant;
            }
            if (rearWheelDrive && !frontWheelDrive)
            {
                backLeftWheelForwardFriction.stiffness = backLeftWheelForwardFriction.stiffness * driftForwardStiffnessConstant;
                backRightWheelForwardFriction.stiffness = backRightWheelForwardFriction.stiffness * driftForwardStiffnessConstant;
            }


            //sideways friction adjustment

            //lerp
            //frontLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, driftFrontSidewaysStiffness, driftBrakesLerpValue);
            //frontRightWheelFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, driftFrontSidewaysStiffness, driftBrakesLerpValue);
            //backLeftWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, driftRearSidewaysStiffness, driftBrakesLerpValue);
            //backRightWheelFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, driftRearSidewaysStiffness, driftBrakesLerpValue);

            //non-lerp
            frontLeftWheelSidewaysFriction.stiffness = driftFrontSidewaysStiffness;
            frontRightWheelSidewaysFriction.stiffness = driftFrontSidewaysStiffness;
            backLeftWheelSidewaysFriction.stiffness = driftRearSidewaysStiffness;
            backRightWheelSidewaysFriction.stiffness = driftRearSidewaysStiffness;

            frontLeftWheelCollider.sidewaysFriction = frontLeftWheelSidewaysFriction;
            frontRightWheelCollider.sidewaysFriction = frontRightWheelSidewaysFriction;
            backLeftWheelCollider.sidewaysFriction = backLeftWheelSidewaysFriction;
            backRightWheelCollider.sidewaysFriction = backRightWheelSidewaysFriction;

            //Debug.Log(frontLeftWheelFriction.stiffness);
        }
        else if (!isBraking)
        {
            //lerp
            if (handbrakeSelection == 0)
            {
                frontLeftWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, (driftBrakesLerpValue - .01f));
                frontRightWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, (driftBrakesLerpValue - .01f));
                backLeftWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, (driftBrakesLerpValue - .01f));
                backRightWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, (driftBrakesLerpValue - .01f));
            }
            if (handbrakeSelection == 1 || handbrakeSelection == 2)
            {
                frontLeftWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
                frontRightWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftFrontSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
                backLeftWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
                backRightWheelSidewaysFriction.stiffness = Mathf.Lerp(currentDriftRearSidewaysStiffness, standardSidewaysStiffness, driftBrakesLerpValue);
            }
            

            //non-lerp
            //frontLeftWheelFriction.stiffness = standardSidewaysStiffness;
            //frontRightWheelFriction.stiffness = standardSidewaysStiffness;
            //backLeftWheelFriction.stiffness = standardSidewaysStiffness;
            //backRightWheelFriction.stiffness = standardSidewaysStiffness;

            frontLeftWheelCollider.sidewaysFriction = frontLeftWheelSidewaysFriction;
            frontRightWheelCollider.sidewaysFriction = frontRightWheelSidewaysFriction;
            backLeftWheelCollider.sidewaysFriction = backLeftWheelSidewaysFriction;
            backRightWheelCollider.sidewaysFriction = backRightWheelSidewaysFriction;
        }
    }

    private void HandleSteering()
    {
        //switch (currentGear)
        //{
        //    case 1:
        //        steerAngle = maxSteerAngle;
        //        break;
        //    case 2:
        //        steerAngle = maxSteerAngle;
        //        break;
        //    case 3:
        //        steerAngle = maxSteerAngle * .8f;
        //        break;
        //    case 4:
        //        steerAngle = maxSteerAngle * .6f;
        //        break;
        //    case 5:
        //        steerAngle = maxSteerAngle * .4f;
        //        break;
        //    case 6:
        //        steerAngle = maxSteerAngle * .2f;
        //        break;
        //}

        currentSteerAngle = (frontLeftWheelCollider.steerAngle + frontRightWheelCollider.steerAngle) / 2;

        switch (steeringAngleSelection)
        {
            case 0:
                maxSteerAngle = streetSteerAngle;
                break;
            case 1:
                maxSteerAngle = driftSteerAngle;
                break;
        }

        float velocity = carRigidBody.velocity.magnitude;
        if (!isBraking)
        {
            if (velocity < 10f)
            {
                steerAngle = maxSteerAngle * 2.5f;
            }
            if (velocity >= 10f && velocity < 15f)
            {
                steerAngle = maxSteerAngle * 1.7f;
            }
            if (velocity >= 15f && velocity < 25f)
            {
                steerAngle = maxSteerAngle * 1f;
            }
            if (velocity >= 25f && velocity < 45f)
            {
                steerAngle = maxSteerAngle * .7f;
            }
            if (velocity >= 45f && velocity < 75f)
            {
                steerAngle = maxSteerAngle * .5f;
            }
            if (velocity >= 75f && velocity < 110f)
            {
                steerAngle = maxSteerAngle * .35f;
            }
            if (velocity >= 110f)
            {
                steerAngle = maxSteerAngle * .35f;
            }
        }

        float newSteerAngle = steerAngle * horizontalInput;
        switch (steeringPowerSelection)
        {
            case 0:
                steeringLerpValue = unpoweredSteeringLerpValue;
                break;
            case 1:
                steeringLerpValue = poweredSteeringLerpValue;
                break;
        }
        //if (frontWheelDrive)
        //{
        //    switch (steeringPowerSelection)
        //    {
        //        case 0:
        //            steeringLerpValue = .6f;
        //            break;
        //        case 1:
        //            steeringLerpValue = .9f;
        //            break;
        //    }
        //}
        //else
        //{
        //    switch (steeringPowerSelection)
        //    {
        //        case 0:
        //            steeringLerpValue = .6f;
        //            break;
        //        case 1:
        //            steeringLerpValue = .9f;
        //            break;
        //    }
        //}
        

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
            if (frontWheelDrive && !rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassFrontAir.localPosition, 0.1f);
            }
            if (!frontWheelDrive && rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassRearAir.localPosition, 0.1f);
            }
            if (frontWheelDrive && rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassCenterAir.localPosition, 0.1f);
            }
            
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
            if (frontWheelDrive && !rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassFrontGround.localPosition, 0.9f);
            }
            if (!frontWheelDrive && rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassRearGround.localPosition, 0.9f);
            }
            if (frontWheelDrive && rearWheelDrive)
            {
                centerOfMassCurrent.localPosition = Vector3.Lerp(centerOfMassCurrent.localPosition, centerOfMassCenterGround.localPosition, 0.9f);
            }
        }

        //Debug.Log(centerOfMassCurrent.localPosition);
    }
    private void HandlePassive()
    {
        if (passiveSelection == 1)
        {
            float velocity = Vector3.Dot(carRigidBody.velocity, transform.forward);

            if (carRigidBody.velocity.magnitude >= 50f && Vector3.Dot(carRigidBody.velocity, transform.forward) > -.9f && Physics.Raycast(transform.position, Vector3.down, 200f))
            {
                carRigidBody.useGravity = false;
                carRigidBody.AddForce(0, -10000, 0); //false gravity
                Vector3 newDirection = carRigidBody.velocity.normalized;

                //carRigidBody.transform.LookAt(Vector3.Lerp(carRigidBody.transform.forward, (newPosition + transform.position), .9f));
                //carRigidBody.transform.LookAt(newPosition + transform.position);

                Vector3 nonGlobalUpwardVector = transform.up;
                nonGlobalUpwardVector.y = 0;

                float carBackwardAngle = transform.forward.y;
                if (carBackwardAngle > .2f)
                {
                    carBackwardAngle = .2f;
                }
                if (carBackwardAngle > 0f) // tilt back
                {
                    carRigidBody.AddForce(transform.up * velocity * flightGravity * carBackwardAngle);
                }

                //test, generic upwards force dependent from velocity independent from angle
                float velocityCapped = velocity;
                if (velocityCapped >= 100f)
                {
                    velocityCapped = 100f;
                }
                carRigidBody.AddForce(transform.up * velocityCapped * flightGravity * .15f);

                float carForwardAngle = transform.forward.y;
                if (carForwardAngle < -.5f)
                {
                    carForwardAngle = -.5f;
                }
                if (carForwardAngle < 0f) // tilt forward
                {
                    carRigidBody.AddForce(transform.forward * velocity * flightGravity * -carForwardAngle);
                }

                float carRollAngle = carRigidBody.transform.eulerAngles.z;
                
                if (carRollAngle > 80f && carRollAngle < 180f)
                {
                    carRollAngle = 80f;
                }
                if (carRollAngle < 280f && carRollAngle > 180f)
                {
                    carRollAngle = 280f;
                }


                if (carRollAngle > 15f) // roll side to side
                {
                    carRigidBody.AddForce(nonGlobalUpwardVector * velocity * flightGravity * (carRollAngle * .002f));
                    //carRigidBody.AddForce(transform.up * velocity * flightGravity * (carRollAngle * .0005f));
                }
                if (carRollAngle < 345f) // roll side to side
                {
                    carRigidBody.AddForce(nonGlobalUpwardVector * velocity * flightGravity * ((360f - carRollAngle) * .002f));
                    //carRigidBody.AddForce(transform.up * velocity * flightGravity * ((360f - carRollAngle) * .0005f));
                }
                
                Debug.Log(carBackwardAngle);
            }
            else
            {
                carRigidBody.useGravity = true;
            }

            //handle wing animations
            if (carRigidBody.velocity.magnitude >= 50f)
            {
                if (carEffects.wingsOpen == false)
                {
                    carEffects.AnimateWingsOpen();
                }
            }
            else
            {
                if (carEffects.wingsOpen == true)
                {
                    carEffects.AnimateWingsClose();
                }

            }


            //Debug.Log(flightGravityActual);
            //Debug.Log(transform.forward.y);



            //float upwardsPullPoint = velocity / 50f;

            //float flightGravityPercentage = velocity / 100f;
            //float flightGravityActual = flightGravity * flightGravityPercentage;
            //if (carRigidBody.velocity.magnitude >= 50f && Vector3.Dot(carRigidBody.velocity, transform.forward) > 0)
            //{
            //    carRigidBody.useGravity = false;
            //    carRigidBody.AddForce(0, -10000, 0); //false gravity
            //    carRigidBody.AddForce(transform.up * flightGravityActual);
            //}
            //else
            //{
            //    carRigidBody.useGravity = true;
            //}



            //upwardsPull = flightUpwardsForce * upwardsPullPoint;
            //if (upwardsPull > 60000)
            //{
            //    upwardsPull = 60000;
            //}
            //if (carRigidBody.velocity.magnitude >= 50f && Vector3.Dot(carRigidBody.velocity, transform.forward) > 0)
            //{
            //    carRigidBody.AddForce(transform.up * upwardsPull);
            //}

            //Debug.Log(transform.forward.y);

            //if (Vector3.Distance(transform.forward, Vector3.Normalize(carRigidBody.velocity)) > .02f)
            //{
            //    Vector3 carForward = transform.forward;
            //    Vector3 velocityForward = Vector3.Normalize(carRigidBody.velocity);
            //    transform.forward = Vector3.MoveTowards(carForward, velocityForward, .1f);
            //    //Vector3.Lerp(carForward, velocityForward, .9f);
            //}
        }

        if (passiveSelection == 2)
        {
            regularWheelGroup.SetActive(false);
            bigWheelGroup.SetActive(true);

            frontLeftWheelCollider = frontLeftBigWheelCollider;
            frontRightWheelCollider = frontRightBigWheelCollider;
            backLeftWheelCollider = backLeftBigWheelCollider;
            backRightWheelCollider = backRightBigWheelCollider;

            frontLeftWheelTransform = frontLeftBigWheelTransform;
            frontRightWheelTransform = frontRightBigWheelTransform;
            backLeftWheelTransform = backLeftBigWheelTransform;
            backRightWheelTransform = backRightBigWheelTransform;
        }
        else
        {
            regularWheelGroup.SetActive(true);
            bigWheelGroup.SetActive(false);

            frontLeftWheelCollider = frontLeftRegularWheelCollider;
            frontRightWheelCollider = frontRightRegularWheelCollider;
            backLeftWheelCollider = backLeftRegularWheelCollider;
            backRightWheelCollider = backRightRegularWheelCollider;

            frontLeftWheelTransform = frontLeftRegularWheelTransform;
            frontRightWheelTransform = frontRightRegularWheelTransform;
            backLeftWheelTransform = backLeftRegularWheelTransform;
            backRightWheelTransform = backRightRegularWheelTransform;
        }
    }

    public void EngageHandler(InputAction.CallbackContext context)
    {
        if (useButtonSelection == 1)
        {
            if (touchingGround)
            {
                carEffects.AnimateJump();
                carRigidBody.AddForce(transform.up * jumpForce);
                carRigidBody.AddForce(transform.forward * (jumpForce / 10));
            }
        }
    }
    public void HandleThruster()
    {
        //Air thruster
        if (useButtonSelection == 2 && controls.Player.Engage.ReadValue<float>() > 0 && playerAirThrusterFuel > 0 && airThrusterFlag)
        {
            carRigidBody.AddForce(transform.forward * thrusterForce);
            playerAirThrusterFuel -= airThrusterDrain;
            airBoosting = true;
        }
        else
        {
            if (playerAirThrusterFuel <= maxAirThrusterFuel)
            {
                if (playerAirThrusterFuel <= airThrusterCutoff) { airThrusterFlag = false; } 
                else { airThrusterFlag = true; }

                playerAirThrusterFuel += airThrusterRegen;
            }

            airBoosting = false;
        }

        //Ground booster
        if (useButtonSelection == 3 && controls.Player.Engage.ReadValue<float>() > 0 && playerGroundBoostFuel > 0 && groundBoostFlag)
        {
            if (touchingGround)
            {
                carRigidBody.AddForce(transform.forward * groundBoostForce);
                groundBoosting = true;


                //frontLeftWheelCollider.motorTorque = frontLeftWheelCollider.motorTorque * 10f;
                //frontRightWheelCollider.motorTorque = frontRightWheelCollider.motorTorque * 10f;
                //backLeftWheelCollider.motorTorque = backLeftWheelCollider.motorTorque * 10f;
                //backRightWheelCollider.motorTorque = backRightWheelCollider.motorTorque * 10f;
            }
            else
            {
                groundBoosting = false;
            }
            playerGroundBoostFuel -= groundBoostDrain;
        }
        else
        {
            if (playerGroundBoostFuel <= maxGroundBoostFuel)
            {
                if (playerGroundBoostFuel <= groundBoostCutoff) { groundBoostFlag = false; }
                else { groundBoostFlag = true; }

                playerGroundBoostFuel += groundBoostRegen;
            }

            groundBoosting = false;
        }
    }
    public void PauseHandler(InputAction.CallbackContext context)
    {
        pauseMenu.OnPause();
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
        if (useButtonSelection == 0 || useButtonSelection == 1)
        {
            boostCanvasGroup.alpha = 0;
        }
        if (useButtonSelection == 2)
        {
            boostCanvasGroup.alpha = 1;
            boostMeter.fillAmount = playerAirThrusterFuel / maxAirThrusterFuel;
            if (playerAirThrusterFuel <= airThrusterCutoff)
            {
                boostMeter.color = Color.red;
            }
            else
            {
                boostMeter.color = thatPurpleyColor;
            }
        }
        if (useButtonSelection == 3)
        {
            boostCanvasGroup.alpha = 1;
            boostMeter.fillAmount = playerGroundBoostFuel / maxGroundBoostFuel;
            if (playerGroundBoostFuel <= groundBoostCutoff)
            {
                boostMeter.color = Color.red;
            }
            else
            {
                boostMeter.color = thatPurpleyColor;
            }
        }
    }
    private void HandleCamera()
    {
        if (isCamming)
        {
            cameraTransposer = CMCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        }
        else
        {

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