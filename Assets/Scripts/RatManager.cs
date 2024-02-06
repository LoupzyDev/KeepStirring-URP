using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPositionOffset;
    [SerializeField] private List<GameObject> _ratatouillesPrefabs;

    private void Start()
    {
        // Subscribe to the recipe delivery event
        RecipeManager.OnRatatouilleSpawned += SpawnRatatouille;
    }

    private void OnDisable()
    {
        RecipeManager.OnRatatouilleSpawned -= SpawnRatatouille;
    }

    // Draw on the editor the position of the spawn point
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // Draw a sphere at the spawn position + the offset
        var pos = transform.position + _spawnPositionOffset;
        Gizmos.DrawSphere(pos, 5f);
    }

    private void SpawnRatatouille()
    {
        if (_ratatouillesPrefabs.Count == 0)
        {
            Debug.LogError("No ratatouille prefabs assigned to the RatManager");
            return;
        }

        // Set the ratatouille's spawn position
        var spawnPosition = transform.position + _spawnPositionOffset;
        // Spawn a ratatouille
        var rat = Instantiate(_ratatouillesPrefabs[Random.Range(0, _ratatouillesPrefabs.Count)], spawnPosition,
            Quaternion.identity);

        // Set the ratatouille's spawn to the spawn position (with no offset so it can trigger)
        rat.GetComponent<Ratatouille>().SetSpawnPosition(transform.position);
    }
}