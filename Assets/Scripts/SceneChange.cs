using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    [SerializeField] GameObject yuchur;
    [SerializeField] GameObject comoJugar;
    [SerializeField] GameObject reglas;
    [SerializeField] GameObject controles;
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Game()
    {
        SceneManager.LoadScene("a 2");
    }

    public void GameWin()
    {
        SceneManager.LoadScene("YouWin");
    }
    public void GameLose()
    {
        SceneManager.LoadScene("YouLose");
    }

    public void Introduction()
    {
        SceneManager.LoadScene("Introduction");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NotChur()
    {
        yuchur.SetActive(false);
    }

    public void Yuchur()
    {
        yuchur.SetActive(true);
    }

    public void HowToPlay()
    {
        comoJugar.SetActive(true);
        reglas.SetActive(false);
        controles.SetActive(false);
    }

    public void BaccToMenu()
    {
        comoJugar.SetActive(false);
    }

    public void Rules()
    {
        controles.SetActive(false);
        reglas.SetActive(true);
    }

    public void Controls()
    {
        reglas.SetActive(false);
        controles.SetActive(true);
    }
}
