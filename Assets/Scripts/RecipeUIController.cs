using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecipeUIController : MonoBehaviour
{
    public VisualTreeAsset recipeRowTemplate;
    public Sprite choppedSprite;
    public Sprite cookedSprite;

    private UIDocument uiDocument;
    private VisualElement recipeRoot;
    private bool displayRecipe = true;
    private bool isPressingR = false;


    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        recipeRoot = uiDocument.rootVisualElement.Q<VisualElement>("root");
        // Subscribe to the recipe change event
        RecipeManager.OnRecipeChanged += SetRecipe;
        // Subscribe to the ingredient change event
        RecipeManager.OnIngredientAdded += UpdateIngredient;
    }

    private void OnDisable()
    {
        RecipeManager.OnRecipeChanged -= SetRecipe;
        RecipeManager.OnIngredientAdded -= UpdateIngredient;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        // Toggles the recipe UI on and off when the player presses the "R" key
        if (Input.GetKeyDown(KeyCode.R) && !isPressingR)
            HideRecipe();
        else if (Input.GetKeyUp(KeyCode.R)) isPressingR = false;
    }

    /// <summary>
    ///  Sets the recipe UI to display the given recipe
    /// </summary>
    /// <param name="recipe"></param>
    public void SetRecipe(Recipe recipe)
    {
        // Clear the recipe UI
        ClearRecipeRows();

        // Add each ingredient to the recipe UI
        foreach (var ingredient in recipe.Ingredients)
        {
            Debug.Log("Adding ingredient to recipe UI: " + ingredient.ToString());
            AddRecipeRow(ingredient.Primitive.Name, ingredient.Primitive.SourceImage, ingredient.Quantity,
                ingredient.Chop == ChopState.Chopped, ingredient.Cook == CookState.Cooked);
        }
    }

    /// <summary>
    /// Adds a new row to the recipe UI
    /// </summary>
    /// <param name="ingredientName"></param>
    /// <param name="sprite"></param>
    /// <param name="amount"></param>
    /// <param name="isChopped"></param>
    /// <param name="isCooked"></param>
    public void AddRecipeRow(string ingredientName, Sprite sprite, int amount, bool isChopped, bool isCooked)
    {
        var newRecipeRow = recipeRowTemplate.Instantiate();
        // Add row-image
        var rowImage = newRecipeRow.Q<VisualElement>("row-image");
        rowImage.style.backgroundImage = sprite.texture;

        // Add state image
        var choppedImage = newRecipeRow.Q<VisualElement>("chopped-image");
        if (isChopped) choppedImage.style.backgroundImage = choppedSprite.texture;

        var cookedImage = newRecipeRow.Q<VisualElement>("cooked-image");
        if (isCooked) cookedImage.style.backgroundImage = cookedSprite.texture;

        // hange amount label
        var amountLabel = newRecipeRow.Q<Label>("amount");
        amountLabel.text = amount.ToString();

        // Add a hidden ingredient label to the row
        var ingredientLabel = newRecipeRow.Q<Label>("ingredient");
        ingredientLabel.text = ingredientName;

        // Add the new row to the recipe UI
        recipeRoot.Add(newRecipeRow);
    }

    /// <summary>
    /// Updates the amount of the given ingredient in the recipe UI
    /// </summary>
    /// <param name="ingredient"></param>
    /// <param name="currentAmount"></param>
    public void UpdateIngredient(Ingredient ingredient, int remaining)
    {
        VisualElement rowToUpdate = null;
        VisualElement rowToRemove = null;
        // Find the row that corresponds to the ingredient
        foreach (var row in recipeRoot.Children())
        {
            // Find the corresponding ingredient
            var ingredientLabel = row.Q<Label>("ingredient");
            if (ingredientLabel.text == ingredient.Primitive.Name)
            {
                if (remaining > 0)
                    // Mark the row to be updated
                    rowToUpdate = row;
                else
                    // Mark the row to be removed
                    rowToRemove = row;
            }
        }

        // Update the row if it was found
        if (rowToUpdate != null)
        {
            // Update the amount label
            var amountLabel = rowToUpdate.Q<Label>("amount");
            amountLabel.text = remaining.ToString();
        }

        if (rowToRemove != null)
            // Remove the row
            recipeRoot.Remove(rowToRemove);
    }

    /// <summary>
    /// Clears all the rows in the recipe UI
    /// </summary>
    public void ClearRecipeRows()
    {
        recipeRoot.Clear();
    }

    public void HideRecipe()
    {
        isPressingR = true;
        displayRecipe = !displayRecipe;
        recipeRoot.style.display = displayRecipe ? DisplayStyle.Flex : DisplayStyle.None;
    }
}