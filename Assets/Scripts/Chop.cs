using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;


public class Chop : MonoBehaviour
{
    public Animator ChoppingTableAnimator;
    public Animator KnifeAnimator;
    public float ChopSeconds;
    public float ChoppingVFXDelay;
    public AudioSource ChoppingSound;

    public Vector3 SpawnOffset;

    // VFX to play when chopping
    public VisualEffect ChoppingVFX;

    // Key to press to chop
    public InputAction ChoppingKey;


    // Keep track if the ChoppingKey is being held down
    private bool _isInTrigger = false;
    private bool _isPressingChop = false;
    private float _timer = 0f;
    private bool _isDone = false;
    private bool _isVfxPlaying = false;
    private bool _isValidIngredient = false;
    private GameObject _currentIngredientGameObject;
    private RecipeManager _recipeManager;
    private CraftingManager _craftingManager;

    // Events for when the chopping key is pressed
    public delegate void ChoppingAction();

    public delegate void ChoppingCancelAction();

    public delegate void ChoppingIngredientAction(GameObject ingredient);

    public static event ChoppingAction OnCook;
    public static event ChoppingCancelAction OnCookCancel;
    public static event ChoppingIngredientAction OnCookIngredient;

    private void OnEnable()
    {
        ChoppingKey.Enable();
        ChoppingKey.started += ChopAction;
        ChoppingKey.canceled += ChopCancel;

        TableInteractableTrigger.OnEnter += OnPlayerEnter;
        TableInteractableTrigger.OnExit += OnPlayerExit;

        TableIngredientTrigger.OnEnter += OnIngredientEnter;
        TableIngredientTrigger.OnExit += OnIngredientExit;
    }

    private void OnDisable()
    {
        ChoppingKey.Disable();
        ChoppingKey.started -= ChopAction;
        ChoppingKey.canceled -= ChopCancel;

        TableInteractableTrigger.OnEnter -= OnPlayerEnter;
        TableInteractableTrigger.OnExit -= OnPlayerExit;

        TableIngredientTrigger.OnEnter -= OnIngredientEnter;
        TableIngredientTrigger.OnExit -= OnIngredientExit;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _recipeManager = FindObjectOfType<RecipeManager>();
        _craftingManager = _recipeManager.craftingManager;
    }

    // Update is called once per frame
    private void Update()
    {
        // If the player is not holding down the chop key or is not in the trigger or the ingredient is not valid, return
        if (!_isPressingChop || !_isInTrigger || !_isValidIngredient) return;

        // Increment the timer
        if (!_isDone) _timer += Time.deltaTime;

        // Check for the delay and play the VFX if the timer is greater than the delay
        if (_timer >= ChoppingVFXDelay && !_isVfxPlaying)
        {
            ChoppingVFX.SendEvent("OnPlay");
            _isVfxPlaying = true;
        }

        if (!(_timer >= ChopSeconds) || _isDone) return;
        // When the timer is done, chop the ingredient
        SpawnChoppedIngredient();
    }

    private static bool CanIngredientBeChopped(GameObject ingredientGameObject)
    {
        // Get the ingredient from the ingredient game object
        if (!ingredientGameObject.TryGetComponent<IngredientObject>(out var ingredient)) return false;
        // If the ingredient is not choppable, return
        Debug.Log("Ingredient: " + ingredient.name + " is chop: " + ingredient.Chop);
        return ingredient.Chop != ChopState.Chopped;
    }

    public void ChopAction(InputAction.CallbackContext context)
    {
        if (!_isInTrigger || !_isValidIngredient) return;
        _isPressingChop = true;
        ChoppingTableAnimator.SetBool("Chop", true);
        KnifeAnimator.SetBool("Chop", true);
        if (ChoppingSound) ChoppingSound.Play();
        OnCook?.Invoke();
    }

    public void ChopCancel(InputAction.CallbackContext context)
    {
        _isPressingChop = false;
        _timer = 0f;
        _isDone = false;
        _isVfxPlaying = false;
        Debug.Log("Chopping Player reset");
        ChoppingTableAnimator.SetBool("Chop", false);
        KnifeAnimator.SetBool("Chop", false);
        if (ChoppingSound) ChoppingSound.Stop();
        StopVFX();
        OnCookCancel?.Invoke();
    }

    private void SpawnChoppedIngredient()
    {
        Debug.Log("Chopped");
        _isDone = true;
        StopVFX();
        // Get the current ingredient
        var currentIngredient = _currentIngredientGameObject.TryGetComponent<IngredientObject>(out var ingredient)
            ? ingredient
            : null;
        if (currentIngredient == null) return;
        // Find the craftable for the current ingredient
        var currentCraftable =
            _craftingManager.craftables.Find(craftable => craftable.Primitive.Name == currentIngredient.Primitive.Name);
        if (currentCraftable == null)
        {
            Debug.Log("Craftable not found for " + currentIngredient.Primitive.Name);
            return;
        }

        GameObject newIngredient;
        // Apply the chop state to the ingredient, depending on the current ingredient's cook state
        switch (currentIngredient.Cook)
        {
            case CookState.Raw:
                newIngredient = currentCraftable.ChoppedIngredient;
                break;
            case CookState.Cooked:
                newIngredient = currentCraftable.ChoppedCookedIngredient;
                break;
            case CookState.Burnt:
                newIngredient = currentCraftable.ChoppedBurntIngredient;
                break;
            default:
                Debug.Log("Invalid cook state");
                throw new ArgumentOutOfRangeException();
        }

        ValidateNewIngredient(newIngredient);
        // Spawn the chopped ingredient
        OnCookIngredient?.Invoke(newIngredient);
    }

    private void OnPlayerEnter(Collider other)
    {
        _isInTrigger = true;
    }

    private void OnPlayerExit()
    {
        _isInTrigger = false;
    }

    private void OnIngredientEnter(Collider other)
    {
        ValidateNewIngredient(other.gameObject);
    }

    private void ValidateNewIngredient(GameObject newIngredient)
    {
        _currentIngredientGameObject = newIngredient;
        _isValidIngredient = CanIngredientBeChopped(newIngredient);
        var message =
            $"Ingredient: {newIngredient.name} is {(_isValidIngredient ? "valid" : "invalid")} for chopping";
        Debug.Log(message);
    }

    private void OnIngredientExit()
    {
        _isValidIngredient = false;
        _currentIngredientGameObject = null;
    }

    private void StopVFX()
    {
        ChoppingVFX.SendEvent("OnStop");
    }
}