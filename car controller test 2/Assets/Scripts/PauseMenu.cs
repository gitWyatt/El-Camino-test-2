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

        int axel = PlayerPrefs.GetInt("axelIndex", 0);
        AxleDropDown.value = axel;

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

        //QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {
        Debug.Log(FPSDropDown.value);
        Debug.Log(PlayerPrefs.GetInt("fpsIndex"));
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

    public void SetPoweredAxle(int axleIndex)
    {
        PlayerPrefs.SetInt("axleIndex", axleIndex);

        if (axleIndex == 0f)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = false;
        }
        if (axleIndex == 1f)
        {
            carController.frontWheelDrive = false;
            carController.rearWheelDrive = true;
        }
        if (axleIndex == 2f)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = true;
        }
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
