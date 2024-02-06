using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

internal enum CookVfxState
{
    None,
    Cooking,
    Cooked,
    Burnt
}

public class Cook : MonoBehaviour
{
    public Animator CookingFurnaceAnimator;
    public Animator CookingAnimator;
    public float CookDuration;
    public float BurntDuration;
    public AudioSource CookingSound;
    public AudioSource CookedSound;
    public AudioSource BurntSound;

    public Vector3 SpawnOffset;

    // VFX to play when cooking
    public VisualEffect CookingVfx;

    // VFX to play when the ingredient is cooked
    public VisualEffect CookedVfx;

    // VFX to play when the ingredient is burnt
    public VisualEffect BurntVfx;

    // Key to press to toggle cooking
    public InputAction CookingKey;


    // Keep track if the Cooking Key is pressed
    public bool IsCooking = false;
    private float _timer = 0f;
    private bool _isInTrigger = false;

    private bool _isValidIngredient = false;

    // Flag to check if the cook state has changed to play the audio only once
    private bool _hasCookStateChanged = false;
    private CookVfxState _cookVfxState = CookVfxState.None;
    private GameObject _currentIngredientGameObject;
    private RecipeManager _recipeManager;
    private CraftingManager _craftingManager;

    public delegate void CookingAction();

    public delegate void CookingCancelAction();

    public delegate void CookingIngredientAction(GameObject ingredient);

    public static event CookingAction OnCook;
    public static event CookingCancelAction OnCookCancel;
    public static event CookingIngredientAction OnIngredientCooked;

    private void OnEnable()
    {
        CookingKey.Enable();
        // Toggle the cooking action when the key is pressed
        // If the player is cooking, cancel the cooking action
        // Else, start the cooking action
        CookingKey.started += CookingToggle;


        FurnaceInteractableTrigger.OnEnter += OnPlayerEnter;
        FurnaceInteractableTrigger.OnExit += OnPlayerExit;

        FurnaceIngredientTrigger.OnEnter += OnIngredientEnter;
        FurnaceIngredientTrigger.OnExit += OnIngredientExit;
    }

    private void OnDisable()
    {
        CookingKey.Disable();
        CookingKey.started -= CookingToggle;

        FurnaceInteractableTrigger.OnEnter -= OnPlayerEnter;
        FurnaceInteractableTrigger.OnExit -= OnPlayerExit;

        FurnaceIngredientTrigger.OnEnter -= OnIngredientEnter;
        FurnaceIngredientTrigger.OnExit -= OnIngredientExit;
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
        // If the player is not cooking something,
        // or the player is not in the trigger
        // or the ingredient is not valid, return
        if (!IsCooking || !_isValidIngredient) return;
        // Increment the timer
        _timer += Time.deltaTime;


        // Play the cooking VFX right away
        if (_cookVfxState == CookVfxState.None)
        {
            Debug.Log("Cooking...");
            _cookVfxState = CookVfxState.Cooking;
            _hasCookStateChanged = true;
        }

        // Play the cooked VFX after a delay
        if (_timer >= CookDuration && _cookVfxState == CookVfxState.Cooking)
        {
            Debug.Log("Cooked!");
            _cookVfxState = CookVfxState.Cooked;
            _hasCookStateChanged = true;
            // Change the ingredient's state to cooked
            CookIngredient();
        }

        // Play the burnt VFX after a delay
        if (_timer >= BurntDuration && _cookVfxState == CookVfxState.Cooked)
        {
            Debug.Log("Burnt :(");
            _cookVfxState = CookVfxState.Burnt;
            _hasCookStateChanged = true;
            // Change the ingredient's state to burnt
            BurnIngredient();
        }

        PlayCookAudio();
        PlayCookVfx();
    }

    private void PlayCookAudio()
    {
        // Play the cooking audio
        if (_cookVfxState == CookVfxState.None || !_hasCookStateChanged) return;
        switch (_cookVfxState)
        {
            case CookVfxState.Cooking:
                if (CookingSound && !CookingSound.isPlaying) CookingSound.Play();
                if (CookedSound && CookedSound.isPlaying) CookedSound.Stop();
                if (BurntSound && BurntSound.isPlaying) BurntSound.Stop();
                break;
            case CookVfxState.Cooked:
                if (CookingSound && CookingSound.isPlaying) CookingSound.Stop();
                if (CookedSound && !CookedSound.isPlaying) CookedSound.Play();
                if (BurntSound && BurntSound.isPlaying) BurntSound.Stop();
                break;
            case CookVfxState.Burnt:
                if (CookingSound && CookingSound.isPlaying) CookingSound.Stop();
                if (CookedSound && CookedSound.isPlaying) CookedSound.Stop();
                if (BurntSound && !BurntSound.isPlaying) BurntSound.Play();
                break;
            case CookVfxState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _hasCookStateChanged = false;
    }

    private void PlayCookVfx()
    {
        switch (_cookVfxState)
        {
            case CookVfxState.None:
                break;
            case CookVfxState.Cooking:
                if (CookingVfx != null) CookingVfx.SendEvent("OnPlay");
                break;
            case CookVfxState.Cooked:
                if (CookingVfx != null) CookingVfx.SendEvent("OnStop");
                if (CookedVfx != null) CookedVfx.SendEvent("OnPlay");
                break;
            case CookVfxState.Burnt:
                if (CookingVfx != null) CookingVfx.SendEvent("OnStop");
                if (CookedVfx != null) CookedVfx.SendEvent("OnStop");
                if (BurntVfx != null) BurntVfx.SendEvent("OnPlay");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // Check if the ingredient is valid to be cooked (not already cooked)
    private bool CanIngredientBeCooked(GameObject ingredientGameObject)
    {
        var cookVfxState = CookVfxState.None;
        if (ingredientGameObject == null)
        {
            Debug.Log("Ingredient game object is null");
            return false;
        }

        // Get the ingredient from the ingredient game object
        if (!ingredientGameObject.TryGetComponent<IngredientObject>(out var ingredient))
        {
            Debug.Log("IngredientObject not found for " + ingredientGameObject.name);
            return false;
        }

        cookVfxState = ingredient.Cook switch
        {
            CookState.Raw => CookVfxState.None,
            CookState.Cooked => CookVfxState.Cooked,
            CookState.Burnt => CookVfxState.Burnt,
            _ => cookVfxState
        };
        _cookVfxState = cookVfxState;

        Debug.Log("Ingredient: " + ingredient.name + " cooking state: " + ingredient.Cook);
        // If the ingredient cannot be cooked (Cook has to be Raw or Cooked), return false
        if (ingredient.Cook is CookState.Raw or CookState.Cooked)
        {
            Debug.Log("Ingredient " + ingredient.name + " can be cooked");
            return true;
        }
        else
        {
            Debug.Log("Ingredient " + ingredient.name + " cannot be cooked");
            return false;
        }
    }

    public void CookingToggle(InputAction.CallbackContext context)
    {
        if (IsCooking)
            CookCancel(context);
        else
            CookAction(context);
    }


    public void CookAction(InputAction.CallbackContext context)
    {
        if (!_isInTrigger) return;
        Debug.Log("Cooking Action triggered");
        // Check if the ingredient is valid
        _isValidIngredient = CanIngredientBeCooked(_currentIngredientGameObject);
        // Turn on the flag for the update function
        IsCooking = true;
        // Play the cooking animation
        if (CookingFurnaceAnimator != null) CookingFurnaceAnimator.SetBool("Cook", true);
        if (CookingAnimator != null) CookingAnimator.SetBool("Cook", true);
        OnCook?.Invoke();
    }


    public void CookCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Cooking Cancelled");
        IsCooking = false;
        _timer = 0f;
        _cookVfxState = CookVfxState.None;
        _isValidIngredient = false;
        _hasCookStateChanged = false;
        // Stop all animations
        if (CookingFurnaceAnimator != null) CookingFurnaceAnimator.SetBool("Cook", false);
        if (CookingAnimator != null) CookingAnimator.SetBool("Cook", false);
        // Stop all VFX
        if (CookingVfx != null) CookingVfx.SendEvent("OnStop");
        if (CookedVfx != null) CookedVfx.SendEvent("OnStop");
        if (BurntVfx != null) BurntVfx.SendEvent("OnStop");
        // Stop the cooking sounds
        // Only stop the cooking sound if it is playing (looping)
        if (CookingSound && CookingSound.isPlaying) CookingSound.Stop();

        OnCookCancel?.Invoke();
    }

    private void SpawnCookedIngredient(CookState cookState)
    {
        // Get the current ingredient
        var currentIngredient = _currentIngredientGameObject.TryGetComponent<IngredientObject>(out var ingredient)
            ? ingredient
            : null;
        if (currentIngredient == null)
        {
            Debug.Log("Ingredient not found when spawning cooked ingredient");
            return;
        }

        // If the current cook state is the same as the ingredient's cook state, return
        if (currentIngredient.Cook == cookState)
        {
            Debug.Log("Ingredient already cooked");
            return;
        }

        // Find the craftable for the current ingredient
        var currentCraftable =
            _craftingManager.craftables.Find(craftable => craftable.Primitive.Name == currentIngredient.Primitive.Name);
        if (currentCraftable == null)
        {
            Debug.Log("Craftable not found for " + currentIngredient.Primitive.Name);
            return;
        }

        // Apply the chop state to the ingredient, depending on the cook state and the current ingredient's chop state
        GameObject newIngredient;
        switch (cookState)
        {
            case CookState.Cooked:
                newIngredient = currentIngredient.Chop == ChopState.None
                    ? currentCraftable.CookedIngredient
                    : currentCraftable.ChoppedCookedIngredient;
                break;
            case CookState.Burnt:
                newIngredient = currentIngredient.Chop == ChopState.None
                    ? currentCraftable.BurntIngredient
                    : currentCraftable.ChoppedBurntIngredient;
                break;
            case CookState.Raw:
            default:
                Debug.Log("Invalid cook state");
                return;
        }


        Debug.Log("Spawning " + currentCraftable.Primitive.Name + " with cook state " + cookState);
        _currentIngredientGameObject = newIngredient;
        OnIngredientCooked?.Invoke(newIngredient);
    }

    private void CookIngredient()
    {
        Debug.Log("Ingredient Cooked");
        SpawnCookedIngredient(CookState.Cooked);
    }

    private void BurnIngredient()
    {
        Debug.Log("Ingredient Burnt");
        SpawnCookedIngredient(CookState.Burnt);
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
        if (!CanIngredientBeCooked(other.gameObject)) return;

        _currentIngredientGameObject = other.gameObject;
    }

    private void OnIngredientExit()
    {
        _currentIngredientGameObject = null;
        _cookVfxState = CookVfxState.None;
    }
}