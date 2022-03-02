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

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;

    public TMPro.TMP_Dropdown QualityDropDown;
    public TMPro.TMP_Dropdown AxleDropDown;
    public TMPro.TMP_Dropdown FPSDropDown;
    public TMPro.TMP_Dropdown MotorDropDown;
    public TMPro.TMP_Dropdown TireDropDown;
    public TMPro.TMP_Dropdown SteeringDropDown;

    private void Awake()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();
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
        int quality = PlayerPrefs.GetInt("qualityIndex", 0);
        QualityDropDown.value = quality;

        int axle = PlayerPrefs.GetInt("axleIndex", 0);
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

        int motor = PlayerPrefs.GetInt("motorIndex", 0);
        MotorDropDown.value = motor;

        int tire = PlayerPrefs.GetInt("tireIndex", 0);
        TireDropDown.value = tire;

        int steering = PlayerPrefs.GetInt("steeringIndex", 0);
        SteeringDropDown.value = steering;
        //QualitySettings.vSyncCount = 0;
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
    public void SetSteering(int steeringIndex)
    {
        switch(steeringIndex)
        {
            case 0:
                carController.steeringSelection = 0;
                break;
            case 1:
                carController.steeringSelection = 1;
                break;
            case 2:
                carController.steeringSelection = 2;
                break;
        }

        PlayerPrefs.SetInt("steeringIndex", steeringIndex);
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
