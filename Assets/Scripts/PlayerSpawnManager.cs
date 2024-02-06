using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool _isSinglePlayer = true;

    private void Start()
    {
        NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;

        if (_isSinglePlayer)
        {
            Debug.Log("Spawning single player");
            NetworkManager.Singleton.StartHost();
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        /* you can use this method in your project to customize one of more aspects of the player
         * (I.E: its start position, its character) and to perform additional validation checks. */
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Position = spawnPoint.position;

        Debug.Log("Connection approved");

        // Start the timer
        FindObjectOfType<Countdown>().StartTimer();
    }
}