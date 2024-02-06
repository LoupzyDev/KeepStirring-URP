using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public AudioSource PotionDropAudio;
    private Value potionSpotlight;
    private Countdown clock;

    public delegate void RecipeDelivered();

    public static event RecipeDelivered OnRecipeDelivered;

    private void Start()
    {
        clock = FindObjectOfType<Countdown>();
        potionSpotlight = FindObjectOfType<Value>();
    }

    private void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bottle")) return;
        PotionDropAudio.Play();
        potionSpotlight.TurnOffSpotlight();
        Destroy(collision.gameObject);
        OnRecipeDelivered?.Invoke();
    }
}