using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class RecipeManager : MonoBehaviour
{
    public CraftingManager craftingManager;

    public CauldronMove cauldronMove;

    public List<Recipe> recipes;

    public Recipe currentRecipe;

    public List<Ingredient> currentIngredients;

    public List<GameObject> spawnedIngredientObjects;

    [SerializeField] private int winConditionRecipeCount = 10;

    private int potionCount = 0;

    private RecipeDifficulty currentDifficulty = RecipeDifficulty.VeryEasy;
    private int strikeCount = 0;
    public int maxStrikes = 3;

    public delegate void RecipeChange(Recipe recipe);

    public static event RecipeChange OnRecipeChanged;

    public delegate void RecipeComplete();

    public static event RecipeComplete OnRecipeComplete;

    public delegate void IngredientChange(Ingredient ingredient, int amount);

    public static event IngredientChange OnIngredientAdded;

    public delegate void IngredientStrike();

    public static event IngredientStrike OnIngredientStrike;

    public delegate void SpawnRatatouille();

    public static event SpawnRatatouille OnRatatouilleSpawned;

    public TextMeshProUGUI strikeTxt;

    public TextMeshProUGUI potionTxt;

    public AudioSource pedillo;

    public Animator strikeShake;

    public DumbWays dumbWays;


    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the list of recipes
        recipes = new List<Recipe>();
        // Create a single recipe
        SetRecipe(GenerateRecipe());
    }

    private void OnEnable()
    {
        // Subscribe to the recipe delivery event
        WinCondition.OnRecipeDelivered += SetNextRecipe;
        IngredientSpawner.OnIngredientSpawned += AddSpawnedIngredient;
        Cauldron.OnIngredientAdded += RemoveSpawnedIngredient;
        TableIngredientTrigger.OnIngredientSpawned += ReplaceSpawnedIngredient;
        FurnaceIngredientTrigger.OnIngredientSpawned += ReplaceSpawnedIngredient;
    }

    private void OnDisable()
    {
        WinCondition.OnRecipeDelivered -= SetNextRecipe;
        IngredientSpawner.OnIngredientSpawned -= AddSpawnedIngredient;
        Cauldron.OnIngredientAdded -= RemoveSpawnedIngredient;
        TableIngredientTrigger.OnIngredientSpawned -= ReplaceSpawnedIngredient;
        FurnaceIngredientTrigger.OnIngredientSpawned -= ReplaceSpawnedIngredient;
    }

    // Update is called once per frame
    private void Update()
    {
        strikeTxt.text = $"{strikeCount} / {maxStrikes}";
        potionTxt.text = $"{potionCount} / {winConditionRecipeCount}";
    }

    public void AddSpawnedIngredient(GameObject ingredient)
    {
        spawnedIngredientObjects.Add(ingredient);
    }

    public void RemoveSpawnedIngredient(GameObject ingredient)
    {
        spawnedIngredientObjects.Remove(ingredient);
    }

    public void ReplaceSpawnedIngredient(GameObject previousIngredient, GameObject newIngredient)
    {
        spawnedIngredientObjects.Remove(previousIngredient);
        spawnedIngredientObjects.Add(newIngredient);
    }

    private static bool MatchIngredients(Ingredient i, Ingredient j)
    {
        return i.Primitive.Name == j.Primitive.Name && i.Chop == j.Chop && i.Cook == j.Cook;
    }

    public bool AddIngredient(Ingredient ingredient)
    {
        Debug.Log("Checking ingredient to add: " + ingredient);

        // Check if the player has added blue and red ingredients, and if so, explode
        var hasBlue = currentIngredients.Exists(i => i.Primitive.Color == IngredientColor.Blue);
        var hasRed = currentIngredients.Exists(i => i.Primitive.Color == IngredientColor.Red);
        var currentIngredientColor = ingredient.Primitive.Color;
        if ((hasBlue && currentIngredientColor == IngredientColor.Red) ||
            (hasRed && currentIngredientColor == IngredientColor.Blue))
        {
            Debug.Log("Blue and red ingredients added, exploding");
            dumbWays.DeathByExplosion();
            return false;
        }

        // Check if the ingredient is in the recipe
        if (!currentRecipe.Ingredients.Exists(i => MatchIngredients(i, ingredient)))
        {
            Debug.Log("Ingredient not in recipe: " + ingredient);
            AddStrike();
            return false;
        }

        var ingredientCountInRecipe =
            currentRecipe.Ingredients.Find(i => MatchIngredients(i, ingredient)).Quantity;

        // If the ingredient is already in the cauldron, don't add it again and instead update the quantity
        if (currentIngredients.Exists(i => MatchIngredients(i, ingredient)))
        {
            // If the ingredient is in the recipe, check if the ingredient count has been exceeded
            if (ingredientCountInRecipe <
                currentIngredients.Find(i => MatchIngredients(i, ingredient)).Quantity +
                ingredient.Quantity)
            {
                Debug.Log("Ingredient count exceeded: " + ingredient);
                AddStrike();
                return false;
            }
            else
            {
                // Update the ingredient quantity
                currentIngredients.Find(i => MatchIngredients(i, ingredient)).Quantity +=
                    ingredient.Quantity;

                Debug.Log("Ingredient quantity updated to: " +
                          currentIngredients.Find(i => MatchIngredients(i, ingredient)).Quantity);
            }
        }
        else
        {
            // Add the ingredient to the cauldron
            currentIngredients.Add(ingredient);
            Debug.Log("Ingredient added: " + ingredient);
        }


        var currentIngredientCount =
            currentIngredients.Find(i => MatchIngredients(i, ingredient))
                .Quantity;
        var remainingIngredientCount = ingredientCountInRecipe - currentIngredientCount;
        // Notify the UI that an ingredient has been added to update the recipe UI
        OnIngredientAdded?.Invoke(ingredient, remainingIngredientCount);
        CheckRecipe();
        return true;
    }

    /// <summary>
    /// Set the current recipe, update the UI to show the new recipe, and reset the current ingredients.
    /// </summary>
    public void SetRecipe(Recipe recipe)
    {
        currentRecipe = recipe;
        // Add the recipe to the list of recipes
        recipes.Add(recipe);
        // Reset the current ingredients
        currentIngredients = new List<Ingredient>();
        // Reset the spawned ingredients
        spawnedIngredientObjects = new List<GameObject>();
        OnRecipeChanged?.Invoke(currentRecipe);
        // Spawn a ratatouille
        SpawnRat();
    }

    public Recipe GenerateRecipe()
    {
        Debug.Log($"Generating recipe of difficulty {currentDifficulty}");
        var newRecipe = Recipe.GenerateRecipe(currentDifficulty, craftingManager);
        Debug.Log("New recipe: " + newRecipe);
        return newRecipe;
    }

    //public void GenerateRecipes()
    //{
    //    Debug.Log("Generating recipes...");


    //    recipes = new List<Recipe>
    //    {
    //        Recipe.GenerateRecipe(RecipeDifficulty.Easy, craftingManager),
    //        Recipe.GenerateRecipe(RecipeDifficulty.Medium, craftingManager),
    //        Recipe.GenerateRecipe(RecipeDifficulty.Medium, craftingManager)
    //    };

    //    recipes.ForEach(r => Debug.Log(r.ToString()));
    //}

    private void AddStrike()
    {
        pedillo.Play();
        strikeCount++;
        Debug.Log("Strike count: " + strikeCount);
        OnIngredientStrike?.Invoke();

        if (strikeShake != null) strikeShake.Play("Base Layer.StrikeDeath", 0, 0f);

        if (strikeCount < maxStrikes) return;
        // Lose condition
        Debug.Log("Game over");
        dumbWays.DeathByExplosion();
    }

    public void CheckRecipe()
    {
        Debug.Log("Checking recipe...");
        if (currentRecipe == null)
        {
            Debug.Log("No recipe to check");
            return;
        }


        foreach (var i in currentIngredients)
        {
            Debug.Log("Checking ingredient: " + i);
            var found = false;
            if (currentRecipe.Ingredients.Any(j => MatchIngredients(i, j) && i.Quantity == j.Quantity))
            {
                Debug.Log("Ingredient " + i.Primitive.Name + " found");
                found = true;
            }

            if (found) continue;
            Debug.Log("Ingredient " + i.Primitive.Name + " not found");
            return;
        }

        if (currentIngredients.Count != currentRecipe.Ingredients.Count)
        {
            // Spawn another rat depending on the difficulty
            switch (currentIngredients.Count)
            {
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                    SpawnRat();
                    break;
            }

            Debug.Log("Wrong number of ingredients");
            return;
        }

        Debug.Log("Recipe correct!");
        OnRecipeComplete?.Invoke();
    }


    public void SetNextRecipe()
    {
        //currentRecipeIndex++;
        //if (currentRecipeIndex >= recipes.Count)
        //{
        //    // Win condition
        //    Debug.Log("You win!");
        //    SceneManager.LoadScene("YouWin");
        //}
        //else
        //{

        //}

        // Add a potion to the counter
        potionCount++;


        // Check if we have to increase the difficulty based on the number of potions made
        currentDifficulty = potionCount switch
        {
            0 => RecipeDifficulty.VeryEasy,
            1 => RecipeDifficulty.Easy,
            2 => RecipeDifficulty.Medium,
            4 => RecipeDifficulty.Hard,
            8 => RecipeDifficulty.VeryHard,
            _ => currentDifficulty
        };

        // Generate a new recipe
        SetRecipe(GenerateRecipe());
    }


    public void SpawnRat()
    {
        var spawnDelay = Ratatouille.GetRandomSpawnDelay(currentRecipe.Difficulty);
        Debug.Log("Spawning ratatouille in " + spawnDelay + " seconds");
        StartCoroutine(SpawnRatCoroutine(spawnDelay));
    }

    private IEnumerator SpawnRatCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Spawning ratatouille");
        OnRatatouilleSpawned?.Invoke();
    }

    public GameObject GetRandomIngredient()
    {
        if (spawnedIngredientObjects.Count != 0)
            return spawnedIngredientObjects[Random.Range(0, spawnedIngredientObjects.Count)];
        Debug.Log("No spawned ingredients");
        return null;
    }
}