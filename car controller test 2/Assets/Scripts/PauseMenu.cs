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
        if (pressingPause)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else if (gameIsPaused == false)
            {
                Pause();
            }
            //Debug.Log(pressingPause);
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
}
