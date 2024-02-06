using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cauldron : MonoBehaviour
{
    public AudioSource DestroyAudio;
    public AudioSource DeathAudio;
    public Animator bigSmoke;
    public Animator bigSmoke2;

    private RecipeManager recipeManager;
    private GameObject collisionObject;

    public delegate void CauldronAction(GameObject ingredient);

    public static event CauldronAction OnIngredientAdded;


    // Start is called before the first frame update
    private void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player falls into the cauldron, game over
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player dropped in cauldron");
            if (DeathAudio != null) DeathAudio.Play();
            if (bigSmoke && bigSmoke2 != null)
            {
                bigSmoke.SetBool("NotExploding", false);
                bigSmoke2.SetBool("NotExploding", false);
            }

            StartCoroutine(WaitForDrown());
        }
        // When an ingredient collides with the cauldron, add it to the recipe
        else if (other.gameObject.CompareTag("Ingredient"))
        {
            // Set the collision object to the ingredient so that it cannot trigger again until the ingredient is destroyed
            if (collisionObject ||
                !other.gameObject.TryGetComponent<IngredientObject>(out var ingredientObject)) return;
            collisionObject = other.gameObject;
            Debug.Log("Ingredient triggered with cauldron: " + collisionObject.name);
            // Convert the ingredient object to an ingredient
            Ingredient ingredient = new(ingredientObject);
            recipeManager.AddIngredient(ingredient);
            // Notify subscribers that an ingredient has been added to the cauldron and should be destroyed
            OnIngredientAdded?.Invoke(other.gameObject);
            // Destroy the ingredient object
            Destroy(other.gameObject);
            if (DestroyAudio != null) DestroyAudio.Play();

            collisionObject = null;
        }
    }

    public IEnumerator WaitForDrown()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("YouExplode");
    }
}