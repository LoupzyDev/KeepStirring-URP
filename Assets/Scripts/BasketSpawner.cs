using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketSpawner : MonoBehaviour
{

	/*public string spawnPointTag = "sometag";
	public bool alwaysSpawn = true;

	public List<GameObject> prefabsToSpawn;*/

	public List<GameObject> canastasToSpawn;
	public List<Transform> canastaSpawnPoints;
	public List<GameObject> barrelsToSpawn;
	public List<Transform> barrelSpawnPoints;


	// Start is called before the first frame update
	void Start()
	{
		SpawnIngredients();

		/*GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
		foreach (GameObject spawnPoint in spawnPoints)
		{
			int randomPrefab = Random.Range(0, prefabsToSpawn.Count);
			if (alwaysSpawn)
			{
				GameObject pts = Instantiate(prefabsToSpawn[randomPrefab]);
				pts.transform.position = spawnPoint.transform.position;
			}
			else
			{
				int spawnOrNot = Random.Range(0, 2);
				if (spawnOrNot == 0)
				{
					GameObject pts = Instantiate(prefabsToSpawn[randomPrefab]);
					pts.transform.position = spawnPoint.transform.position;
				}
			}*/
		
	}

	void SpawnIngredients()
	{
		List<Transform> remainingBarrelSpawnPoints = barrelSpawnPoints;
		foreach (GameObject barrel in barrelsToSpawn)
		{
			Transform selectedSpawn1 = remainingBarrelSpawnPoints[Random.Range(0, remainingBarrelSpawnPoints.Count)];
 
		   remainingBarrelSpawnPoints.Remove(selectedSpawn1);
			Instantiate(barrel, selectedSpawn1);
		}

		List<Transform> remainingCanastaSpawnPoints = canastaSpawnPoints;
		foreach (GameObject canasta in canastasToSpawn)
		{
			Transform selectedSpawn = remainingCanastaSpawnPoints[Random.Range(0, remainingCanastaSpawnPoints.Count)];

			remainingCanastaSpawnPoints.Remove(selectedSpawn);
			Instantiate(canasta, selectedSpawn);
		}

	}
}
