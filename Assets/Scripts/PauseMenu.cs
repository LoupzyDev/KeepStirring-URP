using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PauseMenuState
{
    Paused,
    NotPaused
}

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject yuchur;
    public RecipeUIController recipeUIC;

    public static PauseMenuState State = PauseMenuState.NotPaused;


    // Start is called before the first frame update
    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || !pauseMenu) Pause();

        //else if(Input.GetKeyDown(KeyCode.Escape) || pauseMenu)
        //{
        //    Resume();
        //}
    }

    private void Pause()
    {
        State = PauseMenuState.Paused;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        yuchur.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        recipeUIC.HideRecipe();
    }

    public void Resume()
    {
        State = PauseMenuState.NotPaused;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenu()
    {
        State = PauseMenuState.NotPaused;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public void NotChur()
    {
        yuchur.SetActive(false);
    }

    public void Yuchur()
    {
        yuchur.SetActive(true);
    }
}