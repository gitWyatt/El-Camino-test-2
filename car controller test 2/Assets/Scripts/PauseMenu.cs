using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    CarController carController;
    bool gameIsPaused;
    //bool pressingPause;

    public GameObject pauseMenuUI;

    public GameObject pauseFirstButton;

    private void Start()
    {
        carController = GameObject.Find("Camino").GetComponent<CarController>();
    }

    //public static bool GameIsPaused = false;
    //GameObject go = GameObject.Find("Camino");
    //public CarController carController = go.GetComponent<CarController>();
    //CarController carController = car.GetComponent<CarController>();
    //bool GameIsPaused = CarController.isPausing;

    void Update()
    {
        //pressingPause = carController.pressingPause;
        //if (pressingPause)
        //{
        //    if (gameIsPaused)
        //    {
        //        Resume();
        //    }
        //    else
        //    {
        //        Pause();
        //    }
        //}
    }

    public void OnPause()
    {
        if (gameIsPaused)
        {
            Resume();
            gameIsPaused = false;
        }
        else
        {
            Pause();
            gameIsPaused = true;
        }
        Debug.Log("step 2");
        Debug.Log(gameIsPaused);
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        //gameIsPaused = !gameIsPaused;
        //gameIsPaused = false;
        Debug.Log("step 3");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        Debug.Log("step 3");
    }


    public void LoadDirtTrack()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    public void LoadSkatePark()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
