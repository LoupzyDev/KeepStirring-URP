using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ray Trigger Manager
// --------------------------------
// Creates a single ray from player
// Checks if the ray hits a target within a defined layer
// Notifies subscribed objects of the hit
public class RayTriggerManager : MonoBehaviour
{
    [SerializeField] float maxRayDistance = 300f;
    [SerializeField] LayerMask tooltipLayer;

    // Tooltip events
    public delegate void TooltipDelegate(GameObject tooltipObject);
    public static event TooltipDelegate OnTooltipEnter;
    public delegate void TooltipExitDelegate();
    public static event TooltipExitDelegate OnTooltipExit;

    // Spawnable events
    //public delegate void SpawnableDelegate(GameObject spawnableObject);
    //public static event SpawnableDelegate OnSpawnableEnter;
    //public delegate void SpawnableExitDelegate();
    //public static event SpawnableExitDelegate OnSpawnableExit;

    public static Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {

        RaycastHit hit;

        // For debugging purposes paint the ray
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * maxRayDistance, Color.yellow);

        // Tooltip trigger raycast
        // Multicast delegate to notify subscribed tooltip objects to show the UI element above it.
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxRayDistance, tooltipLayer))
        {
            OnTooltipEnter?.Invoke(hit.collider.gameObject);
        }
        else
        {
            OnTooltipExit?.Invoke();
        }

        // Spawnable trigger raycast
        // Multicast delegate to notify subscribed spawnable objects to spawn an ingredient.
        //    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxRayDistance, spawnableLayer))
        //    {
        //        // Get the instance of the IngredientSpawner of the object that was hit
        //        IngredientSpawner ingredientSpawner = hit.collider.gameObject.GetComponent<IngredientSpawner>();
        //        // Spawn the ingredient
        //        ingredientSpawner.SpawnIngredient();
        //    }
        //    else
        //    {
        //        OnSpawnableExit?.Invoke();
        //    }
    }
}
