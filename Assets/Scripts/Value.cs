using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Value : MonoBehaviour
{
    private Countdown countdown;
    [SerializeField] private List<GameObject> bottles;
    [SerializeField] private bool _resetCountdownOnRecipeComplete = false;

    public static int value = 0;

    public GameObject potionSpotlight;

    public AudioSource spawnAudio;

    //countdown.TimeLeft -= 10;


    private void Start()
    {
        RecipeManager.OnRecipeComplete += NextRecipe;

        countdown = FindObjectOfType<Countdown>();
        potionSpotlight.SetActive(false);
    }

    private void OnDisable()
    {
        RecipeManager.OnRecipeComplete -= NextRecipe;
    }


    public void TurnOffSpotlight()
    {
        potionSpotlight.SetActive(false);
    }


    public void NextRecipe()
    {
        if (bottles.Count == 0)
        {
            Debug.LogError("No bottles found in the list.");
            return;
        }

        Debug.Log("Next Recipe");
        // Get a random bottle from the list
        var bottle = bottles[Random.Range(0, bottles.Count)];
        Debug.Log(bottle.name);
        // Instantiate the bottle
        // Bug: When the scene is reloaded, the object of type 'Value' is destroyed but you are still trying to access it.
        Instantiate(bottle, transform.position, Quaternion.identity);
        spawnAudio.Play();
        potionSpotlight.SetActive(true);

        if (_resetCountdownOnRecipeComplete)
        {
            Debug.Log("Resetting countdown");
            countdown.SetMaxTimeAndStart();
        }
    }
}