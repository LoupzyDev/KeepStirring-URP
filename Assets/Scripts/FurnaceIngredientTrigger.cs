using UnityEngine;

public class FurnaceIngredientTrigger : MonoBehaviour
{
    public AnimationCurve Curve;
    public Vector3 AlignOffset;
    public Vector3 SpawnOffset;

    // Events for triggers
    public delegate void TriggerEnter(Collider other);

    public static event TriggerEnter OnEnter;

    public delegate void TriggerExit();

    public static event TriggerExit OnExit;

    public delegate void IngredientSpawn(GameObject previousIngredient, GameObject newIngredient);

    public static event IngredientSpawn OnIngredientSpawned;

    public float TransitionTime = 1f;

    private GameObject _currentIngredient;
    private Vector3 _targetPosition;
    private bool _magnet = false;
    private float _timer = 0f;
    private Cook _cook;

    private void OnEnable()
    {
        Cook.OnCook += AlignIngredient;
        Cook.OnCookCancel += StopAlignIngredient;
        Cook.OnIngredientCooked += SpawnCookedIngredient;
    }


    private void OnDisable()
    {
        Cook.OnCook -= AlignIngredient;
        Cook.OnCookCancel -= StopAlignIngredient;
        Cook.OnIngredientCooked -= SpawnCookedIngredient;
    }

    private void Start()
    {
        _targetPosition = transform.position + AlignOffset;
        _cook = FindObjectOfType<Cook>();
    }

    private void Update()
    {
        if (!_magnet || _currentIngredient == null || _timer > TransitionTime) return;
        // Move the ingredient to the center of the table with an easing curve
        _timer += Time.deltaTime;
        _currentIngredient.transform.position = Vector3.Lerp(_currentIngredient.transform.position, _targetPosition,
            Curve.Evaluate(_timer / TransitionTime));
    }

    private void AlignIngredient()
    {
        // If the player is in the trigger, magnet the ingredient to the center of the table
        if (_currentIngredient == null) return;
        Debug.Log("Aligning ingredient " + _currentIngredient.name);
        _magnet = true;
        _timer = 0f;
        // Disable the ingredient's rigidbody so it doesn't fall off the table
        _currentIngredient.GetComponent<Rigidbody>().isKinematic = true;
        _currentIngredient.GetComponent<Collider>().enabled = false;
    }

    private void StopAlignIngredient()
    {
        if (_currentIngredient == null) return;
        Debug.Log("Stopping alignment of ingredient " + _currentIngredient.name);
        _magnet = false;
        _currentIngredient.GetComponent<Rigidbody>().isKinematic = false;
        _currentIngredient.GetComponent<Collider>().enabled = true;
    }

    private void SpawnCookedIngredient(GameObject cookedPrefab)
    {
        if (!cookedPrefab)
        {
            Debug.LogError("Trying to spawn a null cooked prefab!");
            return;
        }

        if (_currentIngredient == null)
        {
            Debug.LogError("Trying to spawn a cooked ingredient when there is no current ingredient!");
            return;
        }

        var spawnPosition = transform.position + SpawnOffset;
        var newIngredient = Instantiate(cookedPrefab, spawnPosition, Quaternion.identity);
        OnIngredientSpawned?.Invoke(_currentIngredient, newIngredient);
        Destroy(_currentIngredient);
        _currentIngredient = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ingredient")) return;
        Debug.Log("Ingredient entered trigger" + other.name);
        _currentIngredient = other.gameObject;
        OnEnter?.Invoke(other);
        // Check if we're already cooking something
        if (_cook && _cook.IsCooking) AlignIngredient();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ingredient")) return;
        Debug.Log("Ingredient exited trigger" + other.name);
        _currentIngredient = null;
        OnExit?.Invoke();
    }
}