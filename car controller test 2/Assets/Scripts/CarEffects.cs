using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] public GameObject carBody;

    [SerializeField] public GameObject flWheel;
    [SerializeField] public GameObject frWheel;
    [SerializeField] public GameObject rlWheel;
    [SerializeField] public GameObject rrWheel;

    [SerializeField] public GameObject flBigWheel;
    [SerializeField] public GameObject frBigWheel;
    [SerializeField] public GameObject rlBigWheel;
    [SerializeField] public GameObject rrBigWheel;

    [SerializeField] public TrailRenderer leftAkiraTrailCamino;
    [SerializeField] public TrailRenderer rightAkiraTrailCamino;
    [SerializeField] public TrailRenderer leftAkiraTrailAE86;
    [SerializeField] public TrailRenderer rightAkiraTrailAE86;

    [SerializeField] public ParticleSystem thruster;

    [SerializeField] public GameObject caminoTailLightGroup;
    [SerializeField] public Transform leftTailLight;
    [SerializeField] public Transform rightTailLight;
    [SerializeField] public GameObject ae86TailLightGroup;
    [SerializeField] public Transform leftTailLightAE86;
    [SerializeField] public Transform rightTailLightAE86;

    public Renderer leftTailLightRenderer;
    public Renderer rightTailLightRenderer;

    [SerializeField] public GameObject airThrusters;
    [SerializeField] public GameObject groundBoosters;
    [SerializeField] public GameObject wings;
    [SerializeField] Animator wingsAnimator;

    [SerializeField] public GameObject jumpJacks;
    [SerializeField] Animator jumpAnimator;

    [SerializeField] public GameObject hydraulics;

    [SerializeField] private Mesh elCaminoMesh;
    [SerializeField] private Material elCaminoPaint;
    [SerializeField] private Mesh fumigatorMesh;
    [SerializeField] private Material fumigatorPaint;
    [SerializeField] private Mesh ae86Mesh;
    [SerializeField] private Material ae86Paint;

    [SerializeField] private Mesh offRoadTireMesh;
    [SerializeField] private Material offRoadTirePaint;
    [SerializeField] private Mesh driftTireMesh;
    [SerializeField] private Material driftTirePaint;
    [SerializeField] private Mesh standardTireMesh;
    [SerializeField] private Material standardTirePaint;

    CarController carController;
    
    private bool tireMarksFLFlag;
    private bool tireMarksFRFlag;
    private bool tireMarksBLFlag;
    private bool tireMarksBRFlag;

    [SerializeField] private bool fireMarksSwitch;
    private bool fireMarksFlag;
    [SerializeField] private bool akiraTrailSwitch;
    private bool akiraTrailFlag = true;

    private bool thrusterFlag = false;

    private float sideSlipFL;
    private float sideSlipFR;
    private float sideSlipBL;
    private float sideSlipBR;

    private float frontSlipFL;
    private float frontSlipFR;
    private float frontSlipBL;
    private float frontSlipBR;

    public bool wingsOpen = false;

    public int bodySelection;

    [SerializeField] float sideSlipThreshold;
    [SerializeField] float frontSlipThreshold;

    [SerializeField] public TrailRenderer frontLeftTireMarks;
    [SerializeField] public TrailRenderer frontRightTireMarks;
    [SerializeField] public TrailRenderer backLeftTireMarks;
    [SerializeField] public TrailRenderer backRightTireMarks;

    [SerializeField] public TrailRenderer backLeftFireMarks;
    [SerializeField] public TrailRenderer backRightFireMarks;

    //regular
    [SerializeField] public TrailRenderer frontLeftRegularTireMarks;
    [SerializeField] public TrailRenderer frontRightRegularTireMarks;
    [SerializeField] public TrailRenderer backLeftRegularTireMarks;
    [SerializeField] public TrailRenderer backRightRegularTireMarks;

    [SerializeField] public TrailRenderer backLeftRegularFireMarks;
    [SerializeField] public TrailRenderer backRightRegularFireMarks;

    //big
    [SerializeField] public TrailRenderer frontLeftBigTireMarks;
    [SerializeField] public TrailRenderer frontRightBigTireMarks;
    [SerializeField] public TrailRenderer backLeftBigTireMarks;
    [SerializeField] public TrailRenderer backRightBigTireMarks;

    [SerializeField] public TrailRenderer backLeftBigFireMarks;
    [SerializeField] public TrailRenderer backRightBigFireMarks;

    private void Awake()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();
        var thrusterEmission = thruster.emission;
        thrusterEmission.enabled = false;
        var leftTailLightRenderer = leftTailLight.GetComponent<MeshRenderer>();
        var rightTailLightRenderer = rightTailLight.GetComponent<MeshRenderer>();
        leftTailLightRenderer.enabled = false;
        rightTailLightRenderer.enabled = false;
    }

    private void Update()
    {
        CheckTransforms();
        CheckDrift();
        CheckBoost();
        CheckLights();
    }

    private void CheckTransforms()
    {
        switch (carController.useButtonSelection)
        {
            case 0:
                airThrusters.SetActive(false);
                groundBoosters.SetActive(false);
                jumpJacks.SetActive(false);
                hydraulics.SetActive(false);
                break;
            case 1:
                airThrusters.SetActive(false);
                groundBoosters.SetActive(false);
                jumpJacks.SetActive(true);
                hydraulics.SetActive(true);
                break;
            case 2:
                airThrusters.SetActive(true);
                groundBoosters.SetActive(false);
                jumpJacks.SetActive(false);
                hydraulics.SetActive(false);
                break;
            case 3:
                airThrusters.SetActive(false);
                groundBoosters.SetActive(true);
                jumpJacks.SetActive(false);
                hydraulics.SetActive(false);
                break;
        }

        switch (carController.passiveSelection)
        {
            case 0:
                wings.SetActive(false);
                break;
            case 1:
                wings.SetActive(true);
                break;
            case 2:
                wings.SetActive(false);
                break;
        }

        if (carController.passiveSelection == 2)
        {
            frontLeftTireMarks = frontLeftBigTireMarks;
            frontRightTireMarks = frontRightBigTireMarks;
            backLeftTireMarks = backLeftBigTireMarks;
            backRightTireMarks = backRightBigTireMarks;
            backLeftFireMarks = backLeftBigFireMarks;
            backRightFireMarks = backRightBigFireMarks;
        }
        else
        {
            frontLeftTireMarks = frontLeftRegularTireMarks;
            frontRightTireMarks = frontRightRegularTireMarks;
            backLeftTireMarks = backLeftRegularTireMarks;
            backRightTireMarks = backRightRegularTireMarks;
            backLeftFireMarks = backLeftRegularFireMarks;
            backRightFireMarks = backRightRegularFireMarks;
        }
    }

    private void CheckDrift()
    {
        carController.frontLeftWheelCollider.GetGroundHit(out WheelHit wheelDataFL);
        sideSlipFL = wheelDataFL.sidewaysSlip;
        frontSlipFL = wheelDataFL.forwardSlip;
        carController.frontRightWheelCollider.GetGroundHit(out WheelHit wheelDataFR);
        sideSlipFR = wheelDataFR.sidewaysSlip;
        frontSlipFR = wheelDataFR.forwardSlip;
        carController.backLeftWheelCollider.GetGroundHit(out WheelHit wheelDataBL);
        sideSlipBL = wheelDataBL.sidewaysSlip;
        frontSlipBL = wheelDataBL.forwardSlip;
        carController.backRightWheelCollider.GetGroundHit(out WheelHit wheelDataBR);
        sideSlipBR = wheelDataBR.sidewaysSlip;
        frontSlipBR = wheelDataBR.forwardSlip;

        if (Mathf.Abs(sideSlipFL) > sideSlipThreshold || Mathf.Abs(frontSlipFL) > frontSlipThreshold)
        { startEmitterFL(); } else { stopEmitterFL(); }
        if (Mathf.Abs(sideSlipFR) > sideSlipThreshold || Mathf.Abs(frontSlipFR) > frontSlipThreshold)
        { startEmitterFR(); } else { stopEmitterFR(); }
        if (Mathf.Abs(sideSlipBL) > sideSlipThreshold || Mathf.Abs(frontSlipBL) > frontSlipThreshold)
        { startEmitterBL(); } else { stopEmitterBL(); }
        if (Mathf.Abs(sideSlipBR) > sideSlipThreshold || Mathf.Abs(frontSlipBR) > frontSlipThreshold)
        { startEmitterBR(); } else { stopEmitterBR(); }


        //if (carController.isBraking)
        //{
        //    if (carController.frontLeftWheelCollider.isGrounded)
        //    { startEmitterFL(); } else { stopEmitterFL(); }
        //    if (carController.frontRightWheelCollider.isGrounded)
        //    { startEmitterFR(); } else { stopEmitterFR(); }
        //    if (carController.backLeftWheelCollider.isGrounded)
        //    { startEmitterBL(); } else { stopEmitterBL(); }
        //    if (carController.backRightWheelCollider.isGrounded)
        //    { startEmitterBR(); } else { stopEmitterBR(); }
        //}
        //else
        //{
        //    stopEmitterFL();
        //    stopEmitterFR();
        //    stopEmitterBL();
        //    stopEmitterBR();
        //}
    }

    private void startEmitterFL()
    {
        if (tireMarksFLFlag) return;
        frontLeftTireMarks.emitting = true;
        tireMarksFLFlag = true;
    }
    private void startEmitterFR()
    {
        if (tireMarksFRFlag) return;
        frontRightTireMarks.emitting = true;
        tireMarksFRFlag = true;
    }
    private void startEmitterBL()
    {
        if (tireMarksBLFlag) return;
        backLeftTireMarks.emitting = true;
        tireMarksBLFlag = true;
    }
    private void startEmitterBR()
    {
        if (tireMarksBRFlag) return;
        backRightTireMarks.emitting = true;
        tireMarksBRFlag = true;
    }
    private void stopEmitterFL()
    {
        if (!tireMarksFLFlag) return;
        frontLeftTireMarks.emitting = false;
        tireMarksFLFlag = false;
    }
    private void stopEmitterFR()
    {
        if (!tireMarksFRFlag) return;
        frontRightTireMarks.emitting = false;
        tireMarksFRFlag = false;
    }
    private void stopEmitterBL()
    {
        if (!tireMarksBLFlag) return;
        backLeftTireMarks.emitting = false;
        tireMarksBLFlag = false;
    }
    private void stopEmitterBR()
    {
        if (!tireMarksBRFlag) return;
        backRightTireMarks.emitting = false;
        tireMarksBRFlag = false;
    }

    private void CheckBoost()
    {
        if (carController.groundBoosting && fireMarksSwitch)
        {
            startFireTrail();
        }
        else
        {
            stopFireTrail();
        }

        if (carController.groundBoosting && akiraTrailSwitch)
        {
            startAkiraTrail();
        }
        else
        {
            stopAkiraTrail();
        }

        if (carController.airBoosting)
        {
            startThrusterTrail();
        }
        else
        {
            stopThrusterTrail();
        }
    }

    private void startFireTrail()
    {
        if (fireMarksFlag) return;
        backLeftFireMarks.emitting = true;
        backRightFireMarks.emitting = true;
        fireMarksFlag = true;
    }
    private void stopFireTrail()
    {
        if (!fireMarksFlag) return;
        backLeftFireMarks.emitting = false;
        backRightFireMarks.emitting = false;
        fireMarksFlag = false;
    }
    private void startAkiraTrail()
    {
        switch (bodySelection)
        {
            case 0:
                if (akiraTrailFlag) return;
                leftAkiraTrailCamino.emitting = true;
                rightAkiraTrailCamino.emitting = true;
                akiraTrailFlag = true;
                break;
            case 1:
                if (akiraTrailFlag) return;
                leftAkiraTrailCamino.emitting = true;
                rightAkiraTrailCamino.emitting = true;
                akiraTrailFlag = true;
                break;
            case 2:
                if (akiraTrailFlag) return;
                leftAkiraTrailAE86.emitting = true;
                rightAkiraTrailAE86.emitting = true;
                akiraTrailFlag = true;
                break;
        }
        //if (akiraTrailFlag) return;
        //leftAkiraTrail.emitting = true;
        //rightAkiraTrail.emitting = true;
        //akiraTrailFlag = true;
    }
    private void stopAkiraTrail()
    {
        switch (bodySelection)
        {
            case 0:
                if (!akiraTrailFlag) return;
                leftAkiraTrailCamino.emitting = false;
                rightAkiraTrailCamino.emitting = false;
                akiraTrailFlag = false;
                break;
            case 1:
                if (!akiraTrailFlag) return;
                leftAkiraTrailCamino.emitting = false;
                rightAkiraTrailCamino.emitting = false;
                akiraTrailFlag = false;
                break;
            case 2:
                if (!akiraTrailFlag) return;
                leftAkiraTrailAE86.emitting = false;
                rightAkiraTrailAE86.emitting = false;
                akiraTrailFlag = false;
                break;
        }
        //if (!akiraTrailFlag) return;
        //leftAkiraTrail.emitting = false;
        //rightAkiraTrail.emitting = false;
        //akiraTrailFlag = false;
    }
    private void startThrusterTrail()
    {
        if (thrusterFlag) return;
        var thrusterEmission = thruster.emission;
        thrusterEmission.enabled = true;
        thrusterFlag = true;
    }
    private void stopThrusterTrail()
    {
        if (!thrusterFlag) return;
        var thrusterEmission = thruster.emission;
        thrusterEmission.enabled = false;
        thrusterFlag = false;
    }

    private void CheckLights()
    {
        switch (bodySelection)
        {
            case 0:
                leftTailLightRenderer = leftTailLight.GetComponent<MeshRenderer>();
                rightTailLightRenderer = rightTailLight.GetComponent<MeshRenderer>();
                caminoTailLightGroup.SetActive(true);
                ae86TailLightGroup.SetActive(false);
                break;
            case 1:
                leftTailLightRenderer = leftTailLight.GetComponent<MeshRenderer>();
                rightTailLightRenderer = rightTailLight.GetComponent<MeshRenderer>();
                caminoTailLightGroup.SetActive(true);
                ae86TailLightGroup.SetActive(false);
                break;
            case 2:
                leftTailLightRenderer = leftTailLightAE86.GetComponent<MeshRenderer>();
                rightTailLightRenderer = rightTailLightAE86.GetComponent<MeshRenderer>();
                caminoTailLightGroup.SetActive(false);
                ae86TailLightGroup.SetActive(true);
                break;
        }

        if (carController.isBraking || carController.isReversing || akiraTrailFlag)
        {
            
            leftTailLightRenderer.enabled = true;
            rightTailLightRenderer.enabled = true;
        }
        else
        {
            leftTailLightRenderer.enabled = false;
            rightTailLightRenderer.enabled = false;
        }
    }

    public void AnimateJump()
    {
        if (jumpAnimator != null)
        {
            jumpAnimator.SetTrigger("Jump");
        }
    }
    public void AnimateWingsOpen()
    {
        if (wingsAnimator != null)
        {
            wingsAnimator.SetTrigger("Open");
            wingsOpen = true;
        }
    }
    public void AnimateWingsClose()
    {
        if (wingsAnimator != null)
        {
            wingsAnimator.SetTrigger("Close");
            wingsOpen = false;
        }
    }

    public void SetBodyElCamino()
    {
        carBody.GetComponent<MeshFilter>().mesh = elCaminoMesh;
        carBody.GetComponent<Renderer>().material = elCaminoPaint;

        leftAkiraTrailCamino.emitting = false;
        rightAkiraTrailCamino.emitting = false;
    }
    public void SetBodyFumigator()
    {
        carBody.GetComponent<MeshFilter>().mesh = fumigatorMesh;
        carBody.GetComponent<Renderer>().material = fumigatorPaint;

        leftAkiraTrailCamino.emitting = false;
        rightAkiraTrailCamino.emitting = false;
    }
    public void SetBodyAE86()
    {
        carBody.GetComponent<MeshFilter>().mesh = ae86Mesh;
        carBody.GetComponent<Renderer>().material = ae86Paint;

        leftAkiraTrailAE86.emitting = false;
        rightAkiraTrailAE86.emitting = false;
    }

    public void SetOffRoadTire()
    {
        flWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        flWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        frWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        frWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        rlWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        rlWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        rrWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        rrWheel.GetComponent<Renderer>().material = offRoadTirePaint;

        flBigWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        flBigWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        frBigWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        frBigWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        rlBigWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        rlBigWheel.GetComponent<Renderer>().material = offRoadTirePaint;
        rrBigWheel.GetComponent<MeshFilter>().mesh = offRoadTireMesh;
        rrBigWheel.GetComponent<Renderer>().material = offRoadTirePaint;
    }
    public void SetDriftTire()
    {
        flWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        flWheel.GetComponent<Renderer>().material = driftTirePaint;
        frWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        frWheel.GetComponent<Renderer>().material = driftTirePaint;
        rlWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        rlWheel.GetComponent<Renderer>().material = driftTirePaint;
        rrWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        rrWheel.GetComponent<Renderer>().material = driftTirePaint;

        flBigWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        flBigWheel.GetComponent<Renderer>().material = driftTirePaint;
        frBigWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        frBigWheel.GetComponent<Renderer>().material = driftTirePaint;
        rlBigWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        rlBigWheel.GetComponent<Renderer>().material = driftTirePaint;
        rrBigWheel.GetComponent<MeshFilter>().mesh = driftTireMesh;
        rrBigWheel.GetComponent<Renderer>().material = driftTirePaint;
    }
    public void SetStandardTire()
    {
        flWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        flWheel.GetComponent<Renderer>().material = standardTirePaint;
        frWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        frWheel.GetComponent<Renderer>().material = standardTirePaint;
        rlWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        rlWheel.GetComponent<Renderer>().material = standardTirePaint;
        rrWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        rrWheel.GetComponent<Renderer>().material = standardTirePaint;

        flBigWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        flBigWheel.GetComponent<Renderer>().material = standardTirePaint;
        frBigWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        frBigWheel.GetComponent<Renderer>().material = standardTirePaint;
        rlBigWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        rlBigWheel.GetComponent<Renderer>().material = standardTirePaint;
        rrBigWheel.GetComponent<MeshFilter>().mesh = standardTireMesh;
        rrBigWheel.GetComponent<Renderer>().material = standardTirePaint;
    }
}
