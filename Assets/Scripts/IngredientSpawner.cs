using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject IngredientPrefab;


    public delegate void IngredientSpawnedAction(GameObject ingredient);

    public static event IngredientSpawnedAction OnIngredientSpawned;

    /// <summary>
    /// Spawn an ingredient in front of the player when the player clicks the left mouse button
    /// </summary>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public void SpawnIngredient(Vector3 spawnPosition)
    {
        Debug.Log("Spawning prefab: " + IngredientPrefab.name + " at position: " + spawnPosition);
        var newIngredient = Instantiate(IngredientPrefab, spawnPosition, Quaternion.identity);
        // Notify that the ingredient has been spawned
        OnIngredientSpawned?.Invoke(newIngredient);
    }
}