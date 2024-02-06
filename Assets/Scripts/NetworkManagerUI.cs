using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(StartServer);
        clientButton.onClick.AddListener(StartClient);
        hostButton.onClick.AddListener(StartHost);
    }

    private void StartServer()
    {
        Debug.Log("Starting server...");
        NetworkManager.Singleton.StartServer();
        HideButtons();
    }

    private void StartClient()
    {
        Debug.Log("Starting client...");
        NetworkManager.Singleton.StartClient();
        HideButtons();
    }

    private void StartHost()
    {
        Debug.Log("Starting host...");
        NetworkManager.Singleton.StartHost();
        HideButtons();
    }

    private void HideButtons()
    {
        serverButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
    }
}