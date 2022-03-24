using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] public GameObject carBody;

    [SerializeField] public TrailRenderer frontLeftTireMarks;
    [SerializeField] public TrailRenderer frontRightTireMarks;
    [SerializeField] public TrailRenderer backLeftTireMarks;
    [SerializeField] public TrailRenderer backRightTireMarks;

    [SerializeField] public TrailRenderer backLeftFireMarks;
    [SerializeField] public TrailRenderer backRightFireMarks;

    [SerializeField] public TrailRenderer leftAkiraTrail;
    [SerializeField] public TrailRenderer rightAkiraTrail;

    [SerializeField] public ParticleSystem thruster;

    [SerializeField] public Transform leftTailLight;
    [SerializeField] public Transform rightTailLight;

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

    [SerializeField] float sideSlipThreshold;
    [SerializeField] float frontSlipThreshold;

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
        if (akiraTrailFlag) return;
        leftAkiraTrail.emitting = true;
        rightAkiraTrail.emitting = true;
        akiraTrailFlag = true;
    }
    private void stopAkiraTrail()
    {
        if (!akiraTrailFlag) return;
        leftAkiraTrail.emitting = false;
        rightAkiraTrail.emitting = false;
        akiraTrailFlag = false;
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
        if (carController.isBraking || carController.isReversing || akiraTrailFlag)
        {
            var leftTailLightRenderer = leftTailLight.GetComponent<MeshRenderer>();
            var rightTailLightRenderer = rightTailLight.GetComponent<MeshRenderer>();
            leftTailLightRenderer.enabled = true;
            rightTailLightRenderer.enabled = true;
        }
        else
        {
            var leftTailLightRenderer = leftTailLight.GetComponent<MeshRenderer>();
            var rightTailLightRenderer = rightTailLight.GetComponent<MeshRenderer>();
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
        //var mesh = carBody.GetComponent<MeshFilter>();
        carBody.GetComponent<MeshFilter>().mesh = elCaminoMesh;
        //mesh = elCaminoMesh;

        //working
        carBody.GetComponent<Renderer>().material = elCaminoPaint;

        //elCaminoMesh = carBody.GetComponent<MeshFilter>();
        //elCaminoMesh.sharedMesh = Resources.Load<Mesh>("El Camino");
        //elCaminoMesh.GetComponent<Renderer>().material = elCaminoPaint;
    }
    public void SetBodyFumigator()
    {
        //var mesh = carBody.GetComponent<MeshFilter>();
        carBody.GetComponent<MeshFilter>().mesh = fumigatorMesh;
        //mesh = fumigatorMesh;

        //working
        carBody.GetComponent<Renderer>().material = fumigatorPaint;

        //fumigatorMesh = carBody.GetComponent<MeshFilter>();
        //fumigatorMesh.sharedMesh = Resources.Load<Mesh>("El Camino.003");
        //fumigatorMesh.GetComponent<Renderer>().material = fumigatorPaint;
    }
}
