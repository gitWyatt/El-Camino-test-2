using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputMaster controls;

    CarController carController;

    CarEffects carEffects;

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;

    public TMPro.TMP_Dropdown BodyDropDown;

    public TMPro.TMP_Dropdown QualityDropDown;
    public TMPro.TMP_Dropdown FPSDropDown;

    public TMPro.TMP_Dropdown AxleDropDown;
    public TMPro.TMP_Dropdown MotorDropDown;
    public TMPro.TMP_Dropdown TransmissionDropDown;
    public TMPro.TMP_Dropdown TireDropDown;
    public TMPro.TMP_Dropdown SteeringPowerDropDown;
    public TMPro.TMP_Dropdown SteeringAngleDropDown;
    public TMPro.TMP_Dropdown SuspensionDropDown;
    public TMPro.TMP_Dropdown HandbrakeDropDown;

    public TMPro.TMP_Dropdown PassiveDropDown;
    public TMPro.TMP_Dropdown UseButtonDropDown;

    private void Awake()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();
        carEffects = GameObject.Find("Camino").GetComponent<CarEffects>();
        controls = new InputMaster();
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        int body = PlayerPrefs.GetInt("bodyIndex", 0);
        BodyDropDown.value = body;

        int quality = PlayerPrefs.GetInt("qualityIndex", 0);
        QualityDropDown.value = quality;

        int axle = PlayerPrefs.GetInt("axleIndex", 1);
        AxleDropDown.value = axle;

        int fps = PlayerPrefs.GetInt("fpsIndex", 1);
        switch (fps)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 72;
                break;
            case 3:
                Application.targetFrameRate = 120;
                break;
            case 4:
                Application.targetFrameRate = 144;
                break;
        }
        FPSDropDown.value = fps;

        int motor = PlayerPrefs.GetInt("motorIndex", 1);
        MotorDropDown.value = motor;

        int transmission = PlayerPrefs.GetInt("transmissionIndex", 1);
        TransmissionDropDown.value = transmission;

        int tire = PlayerPrefs.GetInt("tireIndex", 0);
        TireDropDown.value = tire;

        int steeringPower = PlayerPrefs.GetInt("steeringPowerIndex", 1);
        SteeringPowerDropDown.value = steeringPower;

        int steeringAngle = PlayerPrefs.GetInt("steeringAngleIndex", 0);
        SteeringAngleDropDown.value = steeringAngle;

        int suspension = PlayerPrefs.GetInt("suspensionIndex", 1);
        SuspensionDropDown.value = suspension;

        int handbrake = PlayerPrefs.GetInt("handbrakeIndex", 1);
        HandbrakeDropDown.value = handbrake;

        int passive = PlayerPrefs.GetInt("passiveIndex", 0);
        PassiveDropDown.value = passive;

        int useButton = PlayerPrefs.GetInt("useButtonIndex", 0);
        UseButtonDropDown.value = useButton;

        Debug.Log("fuck");
    }

    private void Update()
    {
        //Debug.Log(FPSDropDown.value);
        //Debug.Log(PlayerPrefs.GetInt("fpsIndex"));
    }

    public void OnPause()
    {
        if (Time.timeScale < 0.1f)
        {
            Resume();
        }
        else
        {
            Pause();
        }
        //Debug.Log("step 2");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        //Debug.Log("step 3");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        //Debug.Log("step 3");
    }


    public void LoadDirtTrack()
    {
        Time.timeScale = 1f;
        //pauseMenuUI.SetActive(false);
        SceneManager.LoadScene(1);
    }
    public void LoadSkatePark()
    {
        Time.timeScale = 1f;
        //pauseMenuUI.SetActive(false);
        SceneManager.LoadScene(2);
    }
    public void LoadHills()
    {
        Time.timeScale = 1f;
        //pauseMenuUI.SetActive(false);
        SceneManager.LoadScene(3);
    }
    public void LoadRoadTest1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(4);
    }
    public void LoadRoadTest2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(5);
    }

    public void SetBody(int bodyIndex)
    {
        switch (bodyIndex)
        {
            case 0:
                carController.bodySelection = 0;
                carEffects.bodySelection = 0;
                carEffects.SetBodyElCamino();
                break;
            case 1:
                carController.bodySelection = 1;
                carEffects.bodySelection = 1;
                carEffects.SetBodyFumigator();
                break;
            case 2:
                carController.bodySelection = 2;
                carEffects.bodySelection = 2;
                carEffects.SetBodyAE86();
                break;
        }

        PlayerPrefs.SetInt("bodyIndex", bodyIndex);
    }
    public void SetPoweredAxle(int axleIndex)
    {
        if (axleIndex == 0)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = false;
        }
        if (axleIndex == 1)
        {
            carController.frontWheelDrive = false;
            carController.rearWheelDrive = true;
        }
        if (axleIndex == 2)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = true;
        }

        PlayerPrefs.SetInt("axleIndex", axleIndex);
    }
    public void SetMotor(int motorIndex)
    {
        switch(motorIndex)
        {
            case 0:
                carController.motorSelection = 0;
                break;
            case 1:
                carController.motorSelection = 1;
                break;
        }

        PlayerPrefs.SetInt("motorIndex", motorIndex);
    }
    public void SetTransmission(int transmissionIndex)
    {
        switch(transmissionIndex)
        {
            case 0:
                carController.transmissionSelection = 0;
                break;
            case 1:
                carController.transmissionSelection = 1;
                break;
            case 2:
                carController.transmissionSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("transmissionIndex", transmissionIndex);
    }
    public void SetTire(int tireIndex)
    {
        switch(tireIndex)
        {
            case 0:
                carController.tireSelection = 0;
                break;
            case 1:
                carController.tireSelection = 1;
                break;
            case 2:
                carController.tireSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("tireIndex", tireIndex);
    }
    public void SetSteeringPower(int steeringPowerIndex)
    {
        switch(steeringPowerIndex)
        {
            case 0:
                carController.steeringPowerSelection = 0;
                break;
            case 1:
                carController.steeringPowerSelection = 1;
                break;
        }

        PlayerPrefs.SetInt("steeringPowerIndex", steeringPowerIndex);
    }
    public void SetSteeringAngle(int steeringAngleIndex)
    {
        switch(steeringAngleIndex)
        {
            case 0:
                carController.steeringAngleSelection = 0;
                break;
            case 1:
                carController.steeringAngleSelection = 1;
                break;
        }

        PlayerPrefs.SetInt("steeringAngleIndex", steeringAngleIndex);
    }
    public void SetSuspension(int suspensionIndex)
    {
        switch(suspensionIndex)
        {
            case 0:
                carController.suspensionSelection = 0;
                break;
            case 1:
                carController.suspensionSelection = 1;
                break;
            case 2:
                carController.suspensionSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("suspensionIndex", suspensionIndex);
    }
    public void SetHandbrake(int handbrakeIndex)
    {
        switch(handbrakeIndex)
        {
            case 0:
                carController.handbrakeSelection = 0;
                break;
            case 1:
                carController.handbrakeSelection = 1;
                break;
            case 2:
                carController.handbrakeSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("handbrakeIndex", handbrakeIndex);
    }
    public void SetPassive(int passiveIndex)
    {
        switch(passiveIndex)
        {
            case 0:
                carController.passiveSelection = 0;
                break;
            case 1:
                carController.passiveSelection = 1;
                break;
            case 2:
                carController.passiveSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("passiveIndex", passiveIndex);
    }
    public void SetUseButton(int useButtonIndex)
    {
        switch(useButtonIndex)
        {
            case 0:
                carController.useButtonSelection = 0;
                break;
            case 1:
                carController.useButtonSelection = 1;
                break;
            case 2:
                carController.useButtonSelection = 2;
                break;
            case 3:
                carController.useButtonSelection = 3;
                break;
        }

        PlayerPrefs.SetInt("useButtonIndex", useButtonIndex);
    }


    public void SetQuality(int qualityIndex)
    {        
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityIndex", qualityIndex);

        //QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualityIndex"));
        
        //QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFPS(int fpsIndex)
    {
        switch(fpsIndex)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 72;
                break;
            case 3:
                Application.targetFrameRate = 120;
                break;
            case 4:
                Application.targetFrameRate = 144;
                break;
        }
        //Application.SetTargetFrameRate(fpsIndex);
        //Application.targetFrameRate = fpsIndex;
        PlayerPrefs.SetInt("fpsIndex", fpsIndex);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
