using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Countdown : MonoBehaviour
{
    [SerializeField] private Animator clocky;
    [SerializeField] private AudioSource tikTak;
    [SerializeField] private DumbWays dumbWays;
    [SerializeField] private float _gameplayMaxTimer;
    [SerializeField] private float _tutorialMaxTime;
    public TextMeshProUGUI TimerTxt;

    private float _maxTime;

    public float Timer;


    // Start is called before the first frame update
    private void Start()
    {
        var currentScene = SceneManager.GetActiveScene();
        var sceneName = currentScene.name;

        _maxTime = sceneName == "Tutorial" ? _tutorialMaxTime : _gameplayMaxTimer;
        SetMaxTimeAndStart();
    }

    // Listen for a player to connect to the server
    public void StartTimer()
    {
        Debug.Log("Countdown started");
        SetMaxTimeAndStart();
    }


    // Update is called once per frame
    private void Update()
    {
        if (Timer > 0)
        {
            UpdateTimer();
            UpdateTimerText(Timer);
        }
        else
        {
            Debug.Log("Time has ended");
            ResetTimer();
            dumbWays.DeathByCountdown();
        }

        // Play the time's up sound and animation when the timer is below 30 seconds
        if (Timer <= 30)
        {
            if (clocky != null) clocky.SetBool("TimeEnding", true);
            tikTak.Play();
        }
        else
        {
            clocky.SetBool("TimeEnding", false);
            tikTak.Stop();
        }
    }

    private void UpdateTimerText(float time)
    {
        time += 1;

        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        TimerTxt.text = $"{minutes:00} : {seconds:00}";
    }

    private void UpdateTimer()
    {
        Timer -= Time.deltaTime;
    }

    public void SetMaxTimeAndStart()
    {
        Timer = _maxTime;
    }

    public void ResetTimer()
    {
        Timer = -1;
    }
}