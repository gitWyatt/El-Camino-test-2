using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;

public class MainMenu : MonoBehaviour
{
    public InputMaster controls;

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;

    public TMPro.TMP_Dropdown QualityDropDown;
    public TMPro.TMP_Dropdown FPSDropDown;

    public int fpsCheck = 60;

    private void Awake()
    {
        controls = new InputMaster();
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {

    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        int quality = PlayerPrefs.GetInt("qualityIndex", 0);
        QualityDropDown.value = quality;

        int fps = PlayerPrefs.GetInt("fpsIndex", 1);
        switch (fps)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                fpsCheck = 30;
                Application.targetFrameRate = fpsCheck;
                break;
            case 1:
                QualitySettings.vSyncCount = 0;
                fpsCheck = 60;
                Application.targetFrameRate = fpsCheck;
                break;
            case 2:
                QualitySettings.vSyncCount = 0;
                fpsCheck = 72;
                Application.targetFrameRate = fpsCheck;
                break;
            case 3:
                QualitySettings.vSyncCount = 0;
                fpsCheck = 120;
                Application.targetFrameRate = fpsCheck;
                break;
            case 4:
                QualitySettings.vSyncCount = 0;
                fpsCheck = 144;
                Application.targetFrameRate = fpsCheck;
                break;
        }
        FPSDropDown.value = fps;

        Debug.Log(fps);
    }

    private void Update()
    {
        //if(Application.targetFrameRate != fpsCheck)
        //{
        //    Application.targetFrameRate = fpsCheck;
        //}

        //Debug.Log(FPSDropDown.value);
        //Debug.Log(PlayerPrefs.GetInt("fpsIndex"));
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
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityIndex", qualityIndex);

        //QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualityIndex"));

        //QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFPS(int fpsIndex)
    {
        QualitySettings.vSyncCount = 0;
        switch (fpsIndex)
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
