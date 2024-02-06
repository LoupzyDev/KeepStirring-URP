using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{

    public Camera[] camArray;
    public Camera currentCamera;
    public int secondsWaity = 10;

    void Start()
    {
        TurnOffCameras();
        currentCamera = camArray[0]; 
        currentCamera.enabled = true;
        StartCoroutine(WaitForCamera()); 
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    IEnumerator WaitForCamera()
    {
        //Debug.Log("popo");
        yield return new WaitForSeconds(secondsWaity);
        int nextCamera = Random.Range(0, camArray.Length);
        currentCamera.enabled = false;
        currentCamera = camArray[nextCamera];
        currentCamera.enabled = true;
        StartCoroutine(WaitForCamera());
        
    }

    void TurnOffCameras()
    {
        foreach (Camera cam in camArray)
        {
            cam.enabled = false;
        }
    }

}
