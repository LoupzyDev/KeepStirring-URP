using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialWInCondition : MonoBehaviour
{
    Countdown clock;
    int win = 0;
    public AudioSource audio;
    ValueTutorial potionSpotlight;

    void Start()
    {
        clock = FindObjectOfType<Countdown>();
        potionSpotlight = FindObjectOfType<ValueTutorial>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("win3"))
        {
            win += 3;
            Debug.Log(win);
            audio.Play();
            potionSpotlight.TurnOffSpotlight();
        }
    }

    private void Update()
    {
        if (win >= 3)
        {
            SceneManager.LoadScene("Youwin");
        }
    }
}
