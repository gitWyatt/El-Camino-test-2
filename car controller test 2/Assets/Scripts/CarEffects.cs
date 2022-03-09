using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] public TrailRenderer frontLeftTireMarks;
    [SerializeField] public TrailRenderer frontRightTireMarks;
    [SerializeField] public TrailRenderer backLeftTireMarks;
    [SerializeField] public TrailRenderer backRightTireMarks;

    CarController carController;
    
    private bool tireMarksFLFlag;
    private bool tireMarksFRFlag;
    private bool tireMarksBLFlag;
    private bool tireMarksBRFlag;

    private float sideSlipFL;
    private float sideSlipFR;
    private float sideSlipBL;
    private float sideSlipBR;

    private float frontSlipFL;
    private float frontSlipFR;
    private float frontSlipBL;
    private float frontSlipBR;

    [SerializeField] float sideSlipThreshold;
    [SerializeField] float frontSlipThreshold;

    private void Awake()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();
    }

    private void Update()
    {
        CheckDrift();
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

        if (sideSlipFL > sideSlipThreshold || frontSlipFL > frontSlipThreshold)
        { startEmitterFL(); } else { stopEmitterFL(); }
        if (sideSlipFR > sideSlipThreshold || frontSlipFR > frontSlipThreshold)
        { startEmitterFR(); } else { stopEmitterFR(); }
        if (sideSlipBL > sideSlipThreshold || frontSlipBL > frontSlipThreshold)
        { startEmitterBL(); } else { stopEmitterBL(); }
        if (sideSlipBR > sideSlipThreshold || frontSlipBR > frontSlipThreshold)
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
}
