using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDisable : MonoBehaviour
{
    // On trigger enter, disable the drag interaction on the object if it is an ingredient
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Ingredient") ||
            !other.gameObject.CompareTag("Bottle")
           ) return;
        Debug.Log("Ingredient entered cauldron, stopping drag and drop");
        // Get player game object
        var playerGameObject = GameObject.FindWithTag("Player");
        if (playerGameObject == null)
        {
            Debug.LogError("Could not find player game object");
            return;
        }

        if (playerGameObject.TryGetComponent<DragAndDrop>(out var dragAndDropComponent))
            dragAndDropComponent.StopDrag();
        else Debug.LogError("Could not find drag and drop component on player");
    }
}