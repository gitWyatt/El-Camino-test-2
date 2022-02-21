using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    CarController carController;
    bool gameIsPaused = false;
    bool pressingPause;

    public GameObject pauseMenuUI;

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
        pressingPause = carController.pressingPause;
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
        }
        else
        {
            Pause();
        }
        Debug.Log("step 2");
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = !gameIsPaused;
        //gameIsPaused = false;
        Debug.Log("step 3");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = !gameIsPaused;
        //gameIsPaused = true;
        Debug.Log("step 3");
    }
}
