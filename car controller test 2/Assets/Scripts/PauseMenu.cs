using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputMaster controls;

    CarController carController;

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;


    private void Awake()
    {
        controls = new InputMaster();
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }

    private void Start()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();

    }

    void Update()
    {

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
    public void SetPoweredAxel(int axelIndex)
    {
        if (axelIndex == 0f)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = false;
        }
        if (axelIndex == 1f)
        {
            carController.frontWheelDrive = false;
            carController.rearWheelDrive = true;
        }
        if (axelIndex == 2f)
        {
            carController.frontWheelDrive = true;
            carController.rearWheelDrive = true;
        }
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
