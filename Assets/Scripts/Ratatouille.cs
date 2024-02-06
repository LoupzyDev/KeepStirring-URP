using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum RatatouilleState
{
    Idle,
    Moving,
    Returning,
    Killed
}


public class Ratatouille : MonoBehaviour
{
    public RatatouilleState ratState { get; private set; } = RatatouilleState.Idle;

    [SerializeField] private float _nearestPointSearchRange = 10f;
    [SerializeField] private AudioSource _deathSound;
    [SerializeField] private AudioSource _spawnSound;
    [SerializeField] private AudioSource _eatSound;

    private NavMeshAgent _agent;
    private Vector3 _spawnPosition;
    private Vector3 _destinationPoint;
    private RecipeManager _recipeManager;
    private GameObject _currentIngredient;
    private bool _canMove = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        IngredientSpawner.OnIngredientSpawned += SetIngredientTarget;
        Cauldron.OnIngredientAdded += ChangeIngredientTarget;
        TableIngredientTrigger.OnIngredientSpawned += ChangeIngredientTarget;
        FurnaceIngredientTrigger.OnIngredientSpawned += ChangeIngredientTarget;
        RecipeManager.OnRecipeComplete += KillEvent;
    }


    private void OnDisable()
    {
        IngredientSpawner.OnIngredientSpawned -= SetIngredientTarget;
        Cauldron.OnIngredientAdded -= ChangeIngredientTarget;
        TableIngredientTrigger.OnIngredientSpawned -= ChangeIngredientTarget;
        FurnaceIngredientTrigger.OnIngredientSpawned -= ChangeIngredientTarget;
        RecipeManager.OnRecipeComplete -= KillEvent;
    }

    private void Start()
    {
        _recipeManager = FindObjectOfType<RecipeManager>();
        if (_spawnSound != null) _spawnSound.Play();
        CheckForIngredient();
    }

    private void Update()
    {
        Debug.Log("Rat is " + ratState);
        if (!_currentIngredient) return;


        // Update the destination point to the ingredient's position, or
        // Set the destination back to the spawn point if the rat is returning
        _destinationPoint = ratState == RatatouilleState.Returning
            ? _spawnPosition
            : _currentIngredient.transform.position;

        // If the rat is idle, don't do anything
        if (ratState == RatatouilleState.Idle || ratState == RatatouilleState.Killed || !_canMove)
            return;

        Debug.Log("Rat is moving");
        MoveTo(_destinationPoint);
    }


    private void MoveTo(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out var hitResult, _nearestPointSearchRange, NavMesh.AllAreas))
        {
            if (_agent == null)
            {
                Debug.LogWarning("Could not find NavMeshAgent");
                return;
            }

            _agent.SetDestination(hitResult.position);
            // Draw a ray from the position to the destination
            Debug.DrawRay(_agent.transform.position, _agent.destination - _agent.transform.position, Color.green, 0.1f);
            // Draw the path
            for (var i = 0; i < _agent.path.corners.Length - 1; i++)
                Debug.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1], Color.blue, 0.1f);
        }
        else
        {
            Debug.LogWarning("No valid position found" + position);
        }
    }

    public void CheckForIngredient()
    {
        // Check if the rat is already moving
        if (ratState == RatatouilleState.Moving) return;

        // Get a random ingredient
        var ingredient = _recipeManager.GetRandomIngredient();
        if (ingredient == null)
        {
            Debug.Log("Could not find a random ingredient");
            return;
        }

        // Set the ingredient as the target
        SetIngredientTarget(ingredient);
    }

    public void SetIngredientTarget(GameObject ingredient)
    {
        Debug.Log("Setting ingredient target: " + ingredient.name);
        _currentIngredient = ingredient;
        MoveToIngredient(ingredient);
    }

    public void ChangeIngredientTarget(GameObject ingredient)
    {
        // Get a new ingredient to move to
        var newIngredient = _recipeManager.GetRandomIngredient();
        if (newIngredient == null)
        {
            Debug.Log("Could not find a new ingredient");
            _currentIngredient = null;
            ratState = RatatouilleState.Returning;
            return;
        }

        SetIngredientTarget(newIngredient);
    }

    private void ChangeIngredientTarget(GameObject previousIngredient, GameObject newIngredient)
    {
        ChangeIngredientTarget(newIngredient);
    }

    public void MoveToIngredient(GameObject ingredient)
    {
        _destinationPoint = ingredient.transform.position;
        var moveDelay = GetRandomSpawnDelay(_recipeManager.currentRecipe.Difficulty);
        var ingredientPrimitive = ingredient.TryGetComponent<IngredientObject>(out var ingredientObject)
            ? ingredientObject.Primitive
            : null;
        if (ingredientPrimitive == null)
        {
            Debug.LogError("Could not find ingredient primitive");
            return;
        }

        Debug.Log("Moving to ingredient " + ingredientPrimitive.Name + " in " + moveDelay + " seconds");
        StartCoroutine(MoveToIngredientCoroutine(moveDelay, ingredientPrimitive));
    }

    private IEnumerator MoveToIngredientCoroutine(float delay, IngredientPrimitive ingredient)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Moving to ingredient " + ingredient.Name + " at position " + _destinationPoint);
        _canMove = true;
        ratState = RatatouilleState.Moving;
        MoveTo(_destinationPoint);
    }

    public void MoveToSpawnPoint()
    {
        Debug.Log("Returning to spawn point");
        ratState = RatatouilleState.Returning;
        _destinationPoint = _spawnPosition;
        MoveTo(_destinationPoint);
    }

    // On collision with an ingredient, eat it, and go back to the spawn point
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with an ingredient
        if (!collision.gameObject.CompareTag("Ingredient")) return;
        Debug.Log("Yummy " + collision.gameObject.name);
        Destroy(collision.gameObject);
        if (_eatSound != null) _eatSound.Play();
        MoveToSpawnPoint();
    }

    // On trigger enter with the spawn point, destroy the rat
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("SpawnPoint")) return;
        Debug.Log("Back to the sewers");
        Destroy(gameObject);
    }

    public void Kill()
    {
        ratState = RatatouilleState.Killed;
        Debug.Log("Rat killed xP");
        _canMove = false;
        if (_deathSound != null) _deathSound.Play();
        Destroy(gameObject);
    }

    private void KillEvent()
    {
        Debug.Log("Rat killed by external event");
        Kill();
    }

    public static float GetRandomSpawnDelay(RecipeDifficulty difficulty)
    {
        return difficulty switch
        {
            RecipeDifficulty.VeryEasy => UnityEngine.Random.Range(7f, 15f),
            RecipeDifficulty.Easy => UnityEngine.Random.Range(5f, 10f),
            RecipeDifficulty.Medium => UnityEngine.Random.Range(3f, 7f),
            RecipeDifficulty.Hard => UnityEngine.Random.Range(1f, 5f),
            RecipeDifficulty.VeryHard => UnityEngine.Random.Range(0.5f, 3f),
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
        Debug.Log("Spawn position set to: " + spawnPosition);
    }
}