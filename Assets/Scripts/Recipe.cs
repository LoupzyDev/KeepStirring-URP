using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public enum ChopState
{
    None,
    Chopped
}

public enum CookState
{
    Raw,
    Cooked,
    Burnt
}


public enum RecipeDifficulty
{
    VeryEasy,
    Easy,
    Medium,
    Hard,
    VeryHard
}

public class Recipe
{
    public List<Ingredient> Ingredients;
    public string Name;
    public RecipeDifficulty Difficulty;

    public Recipe(string name, List<Ingredient> ingredients)
    {
        Name = name;
        Ingredients = ingredients;
    }

    public Recipe(string name, List<Ingredient> ingredients, RecipeDifficulty difficulty)
    {
        Name = name;
        Ingredients = ingredients;
        Difficulty = difficulty;
    }

    public override string ToString()
    {
        var s = Name + ": ";
        return Ingredients.Aggregate(s, (current, i) => current + i.ToString() + ", ");
    }

    /// <summary>
    /// Generates a random recipe based on the difficulty
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="manager"></param>
    /// <returns></returns>
    public static Recipe GenerateRecipe(RecipeDifficulty difficulty, CraftingManager manager)
    {
        var minCookedOrChoppedIngredients = 0;
        var minCookedIngredients = 0;
        var minChoppedIngredients = 0;
        var maxQuantity = 2;
        var isCookAndChop = false;

        int numIngredients;

        switch (difficulty)
        {
            case RecipeDifficulty.VeryEasy:
                numIngredients = 1;
                maxQuantity = 1;
                break;
            case RecipeDifficulty.Easy:
                numIngredients = 2;
                maxQuantity = 1;
                break;
            case RecipeDifficulty.Medium:
                numIngredients = 3;
                minCookedOrChoppedIngredients = 1;
                maxQuantity = 2;
                break;
            case RecipeDifficulty.Hard:
                numIngredients = 5;
                minCookedOrChoppedIngredients = 2;
                maxQuantity = 3;
                break;
            case RecipeDifficulty.VeryHard:
                numIngredients = 7;
                minCookedIngredients = 2;
                minChoppedIngredients = 2;
                maxQuantity = 4;
                isCookAndChop = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
        }

        var ingredientsList = new List<Ingredient>();

        var numCookedIngredients = 0;
        var numChoppedIngredients = 0;
        var numBlues = 0;
        var numReds = 0;

        // Create a list of craftables that we can use to pick the name and image of the ingredient
        // Every ingredient must have a unique craftable
        var craftables = new List<Craftable>(manager.craftables);

        for (var i = 0; i < numIngredients; i++)
        {
            var canCook = false;
            var canChop = false;

            if (isCookAndChop)
            {
                canCook = numCookedIngredients < minCookedIngredients;
                canChop = numChoppedIngredients < minChoppedIngredients;
            }
            else
            {
                var canRoll = numCookedIngredients + numChoppedIngredients < minCookedOrChoppedIngredients;
                if (canRoll)
                {
                    var dice = Random.Range(0, 2);
                    canCook = dice == 0;
                    canChop = dice == 1;
                }
            }

            numCookedIngredients += canCook ? 1 : 0;
            numChoppedIngredients += canChop ? 1 : 0;

            var chop = canChop ? ChopState.Chopped : ChopState.None;
            // Only roll between cooked and raw if we can cook
            var cook = canCook ? CookState.Cooked : CookState.Raw;

            var quantity = Random.Range(1, maxQuantity + 1);

            var index = -1;
            Craftable craftable = null;
            while (index == -1)
            {
                var randomIndex = Random.Range(0, craftables.Count);
                var randomCraftable = craftables[randomIndex];
                // Check if there is a blue or red ingredient already in the recipe
                // If there is a blue, only allow non-red ingredients
                // If there is a red, only allow non-blue ingredients
                if ((numBlues > 0 && randomCraftable.Primitive.Color == IngredientColor.Red) ||
                    (numReds > 0 && randomCraftable.Primitive.Color == IngredientColor.Blue))
                {
                    craftables.RemoveAt(randomIndex);
                    continue;
                }

                index = randomIndex;
                craftable = randomCraftable;
            }

            craftables.RemoveAt(index);

            if (craftable.Primitive.Color == IngredientColor.Blue) numBlues++;
            if (craftable.Primitive.Color == IngredientColor.Red) numReds++;

            // Add the ingredient to the recipe
            ingredientsList.Add(new Ingredient(craftable.Primitive, chop, cook, quantity));
        }

        return new Recipe($"Recipe {difficulty}", ingredientsList, difficulty);
    }
}