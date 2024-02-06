using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableIngredientTrigger : MonoBehaviour
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

    private void OnEnable()
    {
        Chop.OnCook += AlignIngredient;
        Chop.OnCookCancel += StopAlignIngredient;
        Chop.OnCookIngredient += SpawnChoppedIngredient;
    }

    private void OnDisable()
    {
        Chop.OnCook -= AlignIngredient;
        Chop.OnCookCancel -= StopAlignIngredient;
        Chop.OnCookIngredient -= SpawnChoppedIngredient;
    }

    private void Start()
    {
        _targetPosition = transform.position + AlignOffset;
    }

    private void AlignIngredient()
    {
        // If the player is in the trigger, magnet the ingredient to the center of the table
        if (_currentIngredient == null) return;
        _magnet = true;
        _timer = 0f;
        // Disable the ingredient's rigidbody so it doesn't fall off the table
        _currentIngredient.GetComponent<Rigidbody>().isKinematic = true;
        _currentIngredient.GetComponent<Collider>().enabled = false;
    }

    private void StopAlignIngredient()
    {
        if (_currentIngredient == null) return;
        _magnet = false;
        _currentIngredient.GetComponent<Rigidbody>().isKinematic = false;
        _currentIngredient.GetComponent<Collider>().enabled = true;
    }

    private void SpawnChoppedIngredient(GameObject choppedPrefab)
    {
        var spawnPosition = transform.position + SpawnOffset;
        var newIngredient = Instantiate(choppedPrefab, spawnPosition, Quaternion.identity);
        OnIngredientSpawned?.Invoke(_currentIngredient, newIngredient);
        Destroy(_currentIngredient);
        _currentIngredient = null;
    }


    private void Update()
    {
        if (!_magnet || _currentIngredient == null || _timer > TransitionTime) return;
        // Move the ingredient to the center of the table with an easing curve
        _timer += Time.deltaTime;
        _currentIngredient.transform.position = Vector3.Lerp(_currentIngredient.transform.position, _targetPosition,
            Curve.Evaluate(_timer / TransitionTime));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ingredient")) return;
        _currentIngredient = other.gameObject;
        OnEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ingredient")) return;
        _currentIngredient = null;
        OnExit?.Invoke();
    }
}