using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInteractableTrigger : MonoBehaviour
{
    // Events for triggers
    public delegate void TriggerEnter(Collider other);

    public delegate void TriggerExit();

    public static event TriggerEnter OnEnter;
    public static event TriggerExit OnExit;

    private void OnTriggerEnter(Collider other)
    {
        // If the trigger is entered by the player, invoke the event OnEnter
        if (other.CompareTag("Player")) OnEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // If the trigger is exited by the player, invoke the event OnExit
        if (other.CompareTag("Player")) OnExit?.Invoke();
    }
}